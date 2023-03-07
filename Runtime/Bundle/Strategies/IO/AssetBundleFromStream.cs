using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Bundle.Exceptions;
using Depra.Assets.Runtime.Interfaces.Files;
using Depra.Assets.Runtime.Interfaces.Requests;
using Depra.Assets.Runtime.Interfaces.Strategies;
using Depra.Coroutines.Runtime;
using UnityEngine;

namespace Depra.Assets.Runtime.Bundle.Strategies.IO
{
    public sealed class AssetBundleFromStream : AssetFileSource<AssetBundle>
    {
        private readonly ICoroutineHost _coroutineHost;

        public AssetBundleFromStream(ICoroutineHost coroutineHost) =>
            _coroutineHost = coroutineHost ?? throw new ArgumentNullException(nameof(coroutineHost));

        public override AssetBundle Load(IAssetFile assetFile)
        {
            using var fileStream = new FileStream(assetFile.Path, FileMode.Open, FileAccess.Read);
            var loadedAssetBundle = AssetBundle.LoadFromStream(fileStream);

            return loadedAssetBundle;
        }

        protected override ITypedAssetRequest<AssetBundle> CreateRequest(IAssetFile assetFile) =>
            new Request(assetFile.Path, _coroutineHost);

        private sealed class Request : ITypedAssetRequest<AssetBundle>, IUntypedAssetRequest
        {
            private readonly string _filePath;
            private readonly ICoroutineHost _coroutineHost;

            private ICoroutine _loadCoroutine;
            private AssetBundleCreateRequest _createRequest;

            public Request(string filePath, ICoroutineHost coroutineHost)
            {
                _filePath = filePath;
                _coroutineHost = coroutineHost;
            }

            public bool Done => _createRequest is { isDone: true };
            public bool Running => _loadCoroutine != null && _createRequest.isDone == false;

            public void Send(IAssetLoadingCallbacks<AssetBundle> callbacks)
            {
                using var fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
                _createRequest = AssetBundle.LoadFromStreamAsync(fileStream);
                _loadCoroutine = _coroutineHost.StartCoroutine(RequestCoroutine(_createRequest, callbacks));
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

            private IEnumerator RequestCoroutine(AssetBundleCreateRequest createRequest,
                IAssetLoadingCallbacks<AssetBundle> callbacks)
            {
                while (createRequest.isDone == false)
                {
                    callbacks.InvokeProgressEvent(createRequest.progress);
                    yield return null;
                }

                callbacks.InvokeLoadedEvent(createRequest.assetBundle);
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

                if (_createRequest.assetBundle != null)
                {
                    _createRequest.assetBundle.Unload(true);
                }
            }

            void IUntypedAssetRequest.Send(IAssetLoadingCallbacks callbacks) => Send(callbacks);

            void IDisposable.Dispose() => Clean();
        }
    }
}