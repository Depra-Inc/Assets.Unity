// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using Depra.Assets.Runtime.Async.Tokens;
using Depra.Assets.Runtime.Files.Structs;

namespace Depra.Assets.Runtime.Files.Interfaces
{
    public interface ILoadableAsset<out TAsset> : IAssetFile
    {
        bool IsLoaded { get; }
        
        TAsset Load();

        void Unload();

        IAsyncToken LoadAsync(
            Action<TAsset> onLoaded,
            Action<DownloadProgress> onProgress = null,
            Action<Exception> onFailed = null);
    }
}