using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Files.Bundles.Exceptions;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Coroutines.Domain.Entities;
using UnityEngine;
using UnityEngine.Networking;

namespace Depra.Assets.Runtime.Files.Bundles.Web
{
    public sealed class AssetBundleFromWeb : AssetBundleFile
    {
        private readonly ICoroutineHost _coroutineHost;

        public AssetBundleFromWeb(AssetIdent ident, ICoroutineHost coroutineHost = null) :
            base(ident, coroutineHost) { }

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

        /// <summary>
        /// Loads asset bundle form server.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="AssetBundleLoadingException"></exception>
        protected override IEnumerator LoadingProcess(Action<AssetBundle> onLoaded, Action<float> onProgress = null,
            Action<Exception> onFailed = null)
        {
            using var webRequest = UnityWebRequestAssetBundle.GetAssetBundle(Path);
            webRequest.SendWebRequest();

            while (webRequest.isDone == false)
            {
                onProgress?.Invoke(webRequest.downloadProgress);
                yield return null;
            }

            EnsureRequestResult(webRequest, onFailed);
            var downloadedBundle = DownloadHandlerAssetBundle.GetContent(webRequest);
            onLoaded.Invoke(downloadedBundle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureRequestResult(UnityWebRequest request, Action<Exception> onFailed = null)
        {
            if (request.CanGetResult() == false)
            {
                onFailed?.Invoke(new AssetBundleLoadingException(Name, Path));
            }
        }
    }
}