using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Bundle.Exceptions;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Interfaces.Files;
using Depra.Assets.Runtime.Interfaces.Requests;
using Depra.Assets.Runtime.Utils;
using Depra.Coroutines.Runtime;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Bundle.Files
{
    public sealed class AssetBundleAssetFile : ILoadableAsset, IDisposable
    {
        private readonly TypedAssetIdent _ident;
        private readonly AssetBundleFile _assetBundle;
        private readonly ICoroutineHost _coroutineHost;

        private Object _loadedAsset;

        public AssetBundleAssetFile(TypedAssetIdent ident, AssetBundleFile assetBundle,
            ICoroutineHost coroutineHost = null)
        {
            _ident = ident;
            _assetBundle = assetBundle ?? throw new ArgumentNullException(nameof(assetBundle));
            _coroutineHost = coroutineHost ?? AssetCoroutineHook.Instance;
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

            var loadedAssetBundle = _assetBundle.Load();
            _loadedAsset = loadedAssetBundle.LoadAsset(Name);
            EnsureAsset(_loadedAsset);

            return _loadedAsset;
        }

        public void Unload()
        {
            if (IsLoaded)
            {
                _loadedAsset = null;
            }
        }

        public void LoadAsync(IAssetLoadingCallbacks callbacks)
        {
            if (IsLoaded)
            {
                callbacks.InvokeProgressEvent(1f);
                callbacks.InvokeLoadedEvent(_loadedAsset);
            }

            _assetBundle.LoadAsync(new AssetLoadingCallbacks<AssetBundle>(
                onLoaded: SendRequest,
                onFailed: exception => throw exception));

            void SendRequest(AssetBundle bundle)
            {
                var request = new Request(by: _ident, from: bundle, _coroutineHost);
                request.Send(new ReturnedAssetLoadingCallbacks(
                    asset => _loadedAsset = asset,
                    new GuardedAssetLoadingCallbacks(callbacks, EnsureAsset)));
            }
        }

        public void UnloadAsync()
        {
            if (IsLoaded == false)
            {
                //_assetBundle.UnloadAsync();
                _loadedAsset = null;
            }
        }

        public void Dispose() => Unload();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureAsset(Object asset)
        {
            if (asset == null)
            {
                throw new AssetBundleFileLoadingException(Name, Path);
            }
        }

        public static implicit operator Object(AssetBundleAssetFile assetFile) => assetFile.Load();

        private sealed class Request : IUntypedAssetRequest
        {
            private readonly AssetBundle _bundle;
            private readonly TypedAssetIdent _asset;
            private readonly ICoroutineHost _coroutineHost;

            private ICoroutine _loadCoroutine;

            public Request(TypedAssetIdent by, AssetBundle from, ICoroutineHost coroutineHost)
            {
                _asset = by;
                _bundle = from;
                _coroutineHost = coroutineHost;
            }

            public bool Done => _loadCoroutine == null;
            public bool Running => _loadCoroutine != null;

            public void Send(IAssetLoadingCallbacks callbacks)
            {
                _loadCoroutine = _coroutineHost.StartCoroutine(RequestCoroutine(callbacks));
            }

            private IEnumerator RequestCoroutine(IAssetLoadingCallbacks callbacks)
            {
                var request = _bundle.LoadAssetAsync(_asset.Path, _asset.Type);
                while (request.isDone == false)
                {
                    callbacks.InvokeProgressEvent(request.progress);
                    yield return null;
                }

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
                _coroutineHost.StopCoroutine(_loadCoroutine);
                _loadCoroutine = null;
            }
        }
    }
}