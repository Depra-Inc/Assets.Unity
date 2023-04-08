// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using System.Threading.Tasks;
using Depra.Assets.Runtime.Files.Structs;

namespace Depra.Assets.Runtime.Async.Threads
{
    internal sealed class AssetThread<TAsset> : IAssetThread<TAsset>
    {
        private readonly Task<TAsset> _task;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public AssetThread(Task<TAsset> task)
        {
            _task = task;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start(Action<TAsset> onLoaded, Action<DownloadProgress> onProgress, Action<Exception> onFailed)
        {
            Task.Run(() => _task);
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}