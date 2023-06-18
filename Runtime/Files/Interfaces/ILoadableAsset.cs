// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Runtime.Files.Delegates;

namespace Depra.Assets.Runtime.Files.Interfaces
{
    public interface ILoadableAsset<TAsset> : IAssetFile
    {
        bool IsLoaded { get; }

        TAsset Load();

        void Unload();

        UniTask<TAsset> LoadAsync(DownloadProgressDelegate onProgress = null, CancellationToken cancellationToken = default);
    }
}