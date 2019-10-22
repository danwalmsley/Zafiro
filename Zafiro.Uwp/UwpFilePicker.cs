﻿using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Zafiro.Core;

namespace Zafiro.Uwp.Controls
{
    public class UwpFilePicker : IFilePicker
    {
        public IObservable<ZafiroFile> Pick(string title, string[] extensions, bool allowNone = false)
        {
            var picker = new FileOpenPicker
            {
                CommitButtonText = title,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            foreach (var ext in extensions)
            {
                picker.FileTypeFilter.Add(ext);
            }

            Func<StorageFile, bool> nulls = file => true;
            Func<StorageFile, bool> notNulls = file => true;
            var filter = allowNone ? nulls : notNulls;

            return Observable
                .FromAsync(() => picker.PickSingleFileAsync().AsTask())
                .Where(filter)
                .Select(storageFile => new UwpFile(storageFile));
        }

        public IObservable<ZafiroFile> PickSave(string title, KeyValuePair<string, IList<string>>[] extensions)
        {
            var picker = new FileSavePicker
            {
                CommitButtonText = title,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            foreach (var pair in extensions)
            {
                picker.FileTypeChoices.Add(pair);
            }

            return Observable
                .FromAsync(() => picker.PickSaveFileAsync().AsTask())
                .Where(storageFile => storageFile != null)
                .Select(storageFile => new UwpFile(storageFile));
        }
    }
}