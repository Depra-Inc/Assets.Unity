using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Bundle.Exceptions;
using Depra.Assets.Runtime.Interfaces.Files;
using Depra.Assets.Runtime.Interfaces.Requests;
using Depra.Assets.Runtime.Interfaces.Strategies;
using Depra.Coroutines.Runtime;
using UnityEngine;
using UnityEngine.Networking;

namespace Depra.Assets.Runtime.Bundle.Strategies.Web
{
    public sealed class AssetBundleFromWeb : AssetFileSource<AssetBundle>
    {
        private readonly ICoroutineHost _coroutineHost;

        public AssetBundleFromWeb(ICoroutineHost coroutineHost) =>
            _coroutineHost = coroutineHost ?? throw new ArgumentNullException(nameof(coroutineHost));

        public override AssetBundle Load(IAssetFile assetFile)
        {
            using var request = UnityWebRequestAssetBundle.GetAssetBundle(assetFile.Path);
            request.SendWebRequest();

            while (request.isDone == false)
            {
                // Spinning for Synchronous Behavior (blocking).
            }

            EnsureRequestResult(request, assetFile.Path, exception => throw exception);
            return DownloadHandlerAssetBundle.GetContent(request);
        }

        protected override ITypedAssetRequest<AssetBundle> CreateRequest(IAssetFile assetFile) =>
            new Request(assetFile.Path, _coroutineHost);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EnsureRequestResult(UnityWebRequest request, string uri, Action<Exception> onFailed = null)
        {
            if (request.CanGetResult() == false)
            {
                onFailed?.Invoke(new AssetBundleLoadingException(uri));
            }
        }

        private sealed class Request : ITypedAssetRequest<AssetBundle>, IUntypedAssetRequest
        {
            private readonly string _uri;
            private readonly ICoroutineHost _coroutineHost;

            private ICoroutine _loadCoroutine;
            private UnityWebRequest _webRequest;

            public Request(string uri, ICoroutineHost coroutineHost)
            {
                _uri = uri;
                _coroutineHost = coroutineHost;
            }

            public bool Done => _webRequest is { isDone: true };
            public bool Running => _loadCoroutine != null && Done == false;

            public void Send(IAssetLoadingCallbacks<AssetBundle> callbacks)
            {
                _webRequest = UnityWebRequestAssetBundle.GetAssetBundle(_uri);
                _loadCoroutine = _coroutineHost.StartCoroutine(RequestCoroutine(_webRequest, callbacks));
            }

            public void Cancel()
            {
                if (Running)
                {
                    Clean();
                }
            }

            public void Destroy()
            {
                if (Done || Running)
                {
                    Clean();
                }
            }

            /// <summary>
            /// Loads asset bundle form server.
            /// </summary>
            /// <returns></returns>
            /// <exception cref="AssetBundleLoadingException"></exception>
            private IEnumerator RequestCoroutine(UnityWebRequest request, IAssetLoadingCallbacks<AssetBundle> callbacks)
            {
                request.SendWebRequest();
                while (request.isDone == false)
                {
                    callbacks.InvokeProgressEvent(request.downloadProgress);
                    yield return null;
                }

                var downloadedBundle = DownloadHandlerAssetBundle.GetContent(request);
                callbacks.InvokeLoadedEvent(downloadedBundle);
                request.Dispose();

                _loadCoroutine = null;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void Clean()
            {
                if (_loadCoroutine != null)
                {
                    _coroutineHost.StopCoroutine(_loadCoroutine);
                    _loadCoroutine = null;
                }

                _webRequest?.Dispose();
                _webRequest = null;
            }

            void IUntypedAssetRequest.Send(IAssetLoadingCallbacks callbacks) => Send(callbacks);

            void IDisposable.Dispose() => Clean();
        }
    }
}