using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Async.Operations;
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

        protected override IAsyncLoad<AssetBundle> RequestAsync() =>
            new LoadFromMainThread<AssetBundle>(_coroutineHost, LoadingProcess, CancelRequest);

        private IEnumerator LoadingProcess(Action<AssetBundle> onLoaded, Action<float> onProgress = null,
            Action<Exception> onFailed = null)
        {
            _webRequest = UnityWebRequestAssetBundle.GetAssetBundle(Path);
            _webRequest.SendWebRequest();

            while (_webRequest.isDone == false)
            {
                onProgress?.Invoke(_webRequest.downloadProgress);
                yield return null;
            }

            onProgress?.Invoke(1f);

            EnsureRequestResult(_webRequest, onFailed);
            var downloadedBundle = DownloadHandlerAssetBundle.GetContent(_webRequest);
            onLoaded.Invoke(downloadedBundle);

            _webRequest.Dispose();
        }

        private void CancelRequest() => _webRequest?.Abort();

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