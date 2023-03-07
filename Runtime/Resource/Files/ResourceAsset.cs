using System.Collections;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Interfaces.Files;
using Depra.Assets.Runtime.Interfaces.Requests;
using Depra.Assets.Runtime.Resource.Exceptions;
using Depra.Assets.Runtime.Resource.Loading;
using Depra.Coroutines.Runtime;
using UnityEngine;

namespace Depra.Assets.Runtime.Resource.Files
{
    public sealed class ResourceAsset : ILoadableAsset
    {
        private readonly TypedAssetIdent _ident;
        private readonly ICoroutineHost _coroutineHost;
        private readonly ResourceAssetSyncLoadingStrategy _syncLoading;

        private Object _loadedAsset;

        public ResourceAsset(TypedAssetIdent ident, ResourceAssetSyncLoadingStrategy loading,
            ICoroutineHost coroutineHost = null)
        {
            _ident = ident;
            _syncLoading = loading;
            _coroutineHost = coroutineHost;
        }

        public string Name => _ident.Name;
        public string Path => _ident.Path;

        public bool IsLoaded => _loadedAsset != null;

        public Object Load()
        {
            if (IsLoaded)
            {
                return _loadedAsset;
            }

            var asset = _syncLoading.Load(_ident.Type);
            EnsureAsset(asset);
            _loadedAsset = asset;

            return _loadedAsset;
        }

        public void Unload()
        {
            if (IsLoaded == false)
            {
                return;
            }

            Resources.UnloadAsset(_loadedAsset);
            _loadedAsset = null;
        }
        
        public void LoadAsync(IAssetLoadingCallbacks callbacks)
        {
            if (IsLoaded)
            {
                callbacks.InvokeProgressEvent(1f);
                callbacks.InvokeLoadedEvent(_loadedAsset);
            }

            var assetRequest = new Request(_ident, _coroutineHost);
            assetRequest.Send(new ReturnedAssetLoadingCallbacks(CompleteRequest,
                new GuardedAssetLoadingCallbacks(callbacks, EnsureAsset)));

            void CompleteRequest(Object loadedAsset)
            {
                _loadedAsset = loadedAsset;
                assetRequest.Destroy();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureAsset(Object asset)
        {
            if (asset == null)
            {
                throw new ResourceLoadingException(_ident.Type, Path);
            }
        }

        private sealed class Request : IUntypedAssetRequest
        {
            private readonly TypedAssetIdent _asset;
            private readonly ICoroutineHost _coroutineHost;

            private ICoroutine _coroutine;

            public Request(TypedAssetIdent asset, ICoroutineHost host = null)
            {
                _asset = asset;
                _coroutineHost = host;
            }

            public bool Done { get; private set; }
            public bool Running => _coroutine != null;

            public void Send(IAssetLoadingCallbacks callbacks)
            {
                Done = false;
                _coroutine = _coroutineHost.StartCoroutine(RequestCoroutine(callbacks));
            }

            private IEnumerator RequestCoroutine(IAssetLoadingCallbacks callbacks)
            {
                var request = Resources.LoadAsync(_asset.Path, _asset.Type);
                while (request.isDone == false)
                {
                    callbacks.InvokeProgressEvent(request.progress);
                    yield return null;
                }

                Done = true;
                callbacks.InvokeLoadedEvent(request.asset);
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

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void Clean()
            {
                Done = false;
                _coroutineHost.StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }
    }
}