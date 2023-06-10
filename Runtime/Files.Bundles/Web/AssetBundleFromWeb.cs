// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Runtime.Exceptions;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Assets.Runtime.Files.Idents;
using Depra.Assets.Runtime.Files.Structs;
using UnityEngine;
using UnityEngine.Networking;

namespace Depra.Assets.Runtime.Files.Bundles.Web
{
    public sealed class AssetBundleFromWeb : AssetBundleFile
    {
        private long _contentSize;
        private UnityWebRequest _webRequest;

        public AssetBundleFromWeb(FileSystemAssetIdent ident) : base(ident) { }

        protected override AssetBundle LoadOverride()
        {
            using var request = UnityWebRequestAssetBundle.GetAssetBundle(Path);
            request.SendWebRequest();

            while (request.isDone == false)
            {
                // Spinning for Synchronous Behavior (blocking).
            }

            Guard.AgainstInvalidRequestResult(request,
                (error, url) => new RemoveAssetBundleNotLoadedException(url, error));

            return DownloadHandlerAssetBundle.GetContent(request);
        }

        protected override async UniTask<AssetBundle> LoadAsyncOverride(CancellationToken cancellationToken,
            IProgress<float> progress = null)
        {
            _webRequest = UnityWebRequestAssetBundle.GetAssetBundle(Path);
            await _webRequest.SendWebRequest().ToUniTask(progress, cancellationToken: cancellationToken);

            Guard.AgainstInvalidRequestResult(_webRequest,
                (error, url) => new RemoveAssetBundleNotLoadedException(url, error));
            
            var downloadedBundle = DownloadHandlerAssetBundle.GetContent(_webRequest);

            _contentSize = _webRequest.ParseSize();
            _webRequest.Dispose();

            return downloadedBundle;
        }

        protected override FileSize FindSize(AssetBundle assetBundle) => new(_contentSize);

        private void CancelRequest() => _webRequest?.Abort();
    }
}