// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Unity.Runtime.Exceptions;
using Depra.Assets.Unity.Runtime.Files.Bundles.Files;
using Depra.Assets.Unity.Runtime.Files.Idents;
using Depra.Assets.ValueObjects;
using UnityEngine;
using UnityEngine.Networking;

namespace Depra.Assets.Unity.Runtime.Files.Bundles.Web
{
    public sealed class AssetBundleFromWeb : AssetBundleFile
    {
        private long _contentSize;

        public AssetBundleFromWeb(FileSystemAssetIdent ident) : base(ident) { }

        protected override AssetBundle LoadOverride()
        {
            using var request = UnityWebRequestAssetBundle.GetAssetBundle(Ident.Uri);
            request.SendWebRequest();

            while (request.isDone == false)
            {
                // Spinning for Synchronous Behavior (blocking).
            }

            Guard.AgainstInvalidRequestResult(request,
                (error, url) => new RemoveAssetBundleNotLoadedException(url, error));

            return DownloadHandlerAssetBundle.GetContent(request);
        }

        protected override async UniTask<AssetBundle> LoadAsyncOverride(IProgress<float> progress = null,
            CancellationToken cancellationToken = default)
        {
            var webRequest = UnityWebRequestAssetBundle.GetAssetBundle(Ident.Uri);
            await webRequest
                .SendWebRequest()
                .ToUniTask(progress, cancellationToken: cancellationToken);

            Guard.AgainstInvalidRequestResult(webRequest,
                (error, url) => new RemoveAssetBundleNotLoadedException(url, error));
            
            var downloadedBundle = DownloadHandlerAssetBundle.GetContent(webRequest);

            _contentSize = webRequest.ParseSize();
            webRequest.Dispose();

            return downloadedBundle;
        }

        protected override FileSize FindSize(AssetBundle assetBundle) => new(_contentSize);
    }
}