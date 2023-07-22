// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Delegates;
using Depra.Assets.Files;

namespace Depra.Assets.Unity.Runtime.Files.Adapter
{
    /// <summary>
    /// Represents an interface for a loadable asset.
    /// </summary>
    /// <typeparam name="TAsset">The type of the asset to be loaded.</typeparam>
    public interface IUnityLoadableAsset<TAsset> : IAssetFile
    {
        /// <summary>
        /// Gets a value indicating whether the asset is loaded.
        /// </summary>
        bool IsLoaded { get; }

        /// <summary>
        /// Loads the asset synchronously.
        /// </summary>
        /// <returns>The loaded asset of type <typeparamref name="TAsset"/>.</returns>
        TAsset Load();

        /// <summary>
        /// Unloads the asset.
        /// </summary>
        void Unload();

        /// <summary>
        /// Loads the asset asynchronously.
        /// </summary>
        /// <param name="onProgress">An optional delegate for tracking the download progress.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the async operation.</param>
        /// <returns>A task representing the asynchronous loading operation, returning the loaded asset of type <typeparamref name="TAsset"/>.</returns>
        UniTask<TAsset> LoadAsync(DownloadProgressDelegate onProgress = null,
            CancellationToken cancellationToken = default);
    }
}