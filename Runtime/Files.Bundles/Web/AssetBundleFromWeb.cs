// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Async.Threads;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Assets.Runtime.Files.Structs;
using Depra.Coroutines.Domain.Entities;
using UnityEngine;
using UnityEngine.Networking;

namespace Depra.Assets.Runtime.Files.Bundles.Web
{
    public sealed class AssetBundleFromWeb : AssetBundleFile
    {
        private readonly ICoroutineHost _coroutineHost;

        private long _contentSize;
        private UnityWebRequest _webRequest;

        public AssetBundleFromWeb(AssetIdent ident, ICoroutineHost coroutineHost = null) : base(ident) =>
            _coroutineHost = coroutineHost;

        protected override AssetBundle LoadOverride()
        {
            using var request = UnityWebRequestAssetBundle.GetAssetBundle(Path);
            request.SendWebRequest();

            while (request.isDone == false)
            {
                // Spinning for Synchronous Behavior (blocking).
            }

            EnsureRequestResult(request, exception => throw exception);
            return DownloadHandlerAssetBundle.GetContent(request);
        }

        protected override IAssetThread<AssetBundle> RequestAsync() =>
            new MainAssetThread<AssetBundle>(_coroutineHost, LoadingProcess, CancelRequest);

        protected override FileSize RefreshSize(AssetBundle assetBundle) => new(_contentSize);

        private IEnumerator LoadingProcess(Action<AssetBundle> onLoaded, Action<DownloadProgress> onProgress = null,
            Action<Exception> onFailed = null)
        {
            _webRequest = UnityWebRequestAssetBundle.GetAssetBundle(Path);
            _webRequest.SendWebRequest();

            while (_webRequest.isDone == false)
            {
                var progress = new DownloadProgress(_webRequest.downloadProgress);
                onProgress?.Invoke(progress);

                yield return null;
            }

            onProgress?.Invoke(DownloadProgress.Full);

            EnsureRequestResult(_webRequest, onFailed);
            
            var downloadedBundle = DownloadHandlerAssetBundle.GetContent(_webRequest);
            onLoaded.Invoke(downloadedBundle);

            _contentSize = _webRequest.ParseSize();
            _webRequest.Dispose();
        }

        private void CancelRequest() => _webRequest?.Abort();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureRequestResult(UnityWebRequest request, Action<Exception> onFailed = null)
        {
            if (request.CanGetResult() == false)
            {
                onFailed?.Invoke(new RemoveAssetBundleNotLoadedException(Path, request.error));
            }
        }
    }
}