using System;
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
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Resource.Files
{
    public sealed class ResourceAsset<TAsset> : ILoadableAsset<TAsset> where TAsset : Object
    {
        private readonly AssetIdent _ident;
        private readonly ICoroutineHost _coroutineHost;
        private readonly ResourceAssetSyncLoadingStrategy _loading;

        private TAsset _loadedAsset;

        public ResourceAsset(AssetIdent ident, ResourceAssetSyncLoadingStrategy loading,
            ICoroutineHost coroutineHost = null)
        {
            _ident = ident;
            _loading = loading;
            _coroutineHost = coroutineHost;
        }

        public string Name => _ident.Name;
        public string Path => _ident.Path;

        public bool IsLoaded => _loadedAsset != null;

        public TAsset Load()
        {
            if (IsLoaded)
            {
                return _loadedAsset;
            }

            var asset = _loading.Load<TAsset>();
            EnsureAsset(asset);
            _loadedAsset = asset;

            return asset;
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

        public void LoadAsync(IAssetLoadingCallbacks<TAsset> callbacks)
        {
            if (IsLoaded)
            {
                callbacks.InvokeProgressEvent(1f);
                callbacks.InvokeLoadedEvent(_loadedAsset);
            }

            var assetRequest = new Request(Path, _coroutineHost);
            assetRequest.Send(new ReturnedAssetLoadingCallbacks<TAsset>(CompleteRequest,
                new GuardedAssetLoadingCallbacks<TAsset>(callbacks, EnsureAsset)));

            void CompleteRequest(TAsset loadedAsset)
            {
                _loadedAsset = loadedAsset;
                assetRequest.Destroy();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureAsset(TAsset asset)
        {
            if (asset == null)
            {
                throw new ResourceLoadingException(typeof(TAsset), Path);
            }
        }

        private sealed class Request : ITypedAssetRequest<TAsset>
        {
            private readonly string _assetPath;
            private readonly ICoroutineHost _coroutineHost;

            private ICoroutine _coroutine;

            public Request(string assetPath, ICoroutineHost host = null)
            {
                _assetPath = assetPath;
                _coroutineHost = host;
            }

            public bool Done { get; private set; }
            public bool Running => _coroutine != null;

            public void Send(IAssetLoadingCallbacks<TAsset> callbacks)
            {
                Done = false;
                _coroutine = _coroutineHost.StartCoroutine(RequestCoroutine(callbacks));
            }

            private IEnumerator RequestCoroutine(IAssetLoadingCallbacks<TAsset> callbacks)
            {
                var request = Resources.LoadAsync<TAsset>(_assetPath);
                while (request.isDone == false)
                {
                    callbacks.InvokeProgressEvent(request.progress);
                    yield return null;
                }

                Done = true;
                var assetAsT = (TAsset)request.asset;
                callbacks.InvokeLoadedEvent(assetAsT);
                _coroutine = null;
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

            void IDisposable.Dispose() => Clean();
        }
    }
}