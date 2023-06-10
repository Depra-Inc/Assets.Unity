// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Runtime.Files.Resource;
using Depra.Assets.Runtime.Files.Structs;

namespace Depra.Assets.Runtime.Files.Interfaces
{
    public interface ILoadableAsset<TAsset> : IAssetFile
    {
        bool IsLoaded { get; }

        TAsset Load();

        void Unload();

        UniTask<TAsset> LoadAsync(CancellationToken cancellationToken, DownloadProgressDelegate onProgress = null);
    }
}