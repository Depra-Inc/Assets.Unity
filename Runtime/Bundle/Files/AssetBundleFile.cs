using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Bundle.Exceptions;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Interfaces.Files;
using Depra.Assets.Runtime.Internal.Patterns;
using Depra.Assets.Runtime.Utils;
using Depra.Coroutines.Domain.Entities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Bundle.Files
{
    public abstract class AssetBundleFile : ILoadableAsset<AssetBundle>, IDisposable
    {
        private readonly AssetIdent _ident;
        private readonly ICoroutineHost _coroutineHost;

        private AssetBundle _loadedAssetBundle;

        protected AssetBundleFile(AssetIdent ident, ICoroutineHost coroutineHost = null)
        {
            _ident = ident;
            _coroutineHost = coroutineHost ?? AssetCoroutineHook.Instance;
        }

        public string Name => _ident.Name;
        public string Path => _ident.Path;
        public bool IsLoaded => _loadedAssetBundle != null;

        public TAsset Load<TAsset>(string name) where TAsset : Object
        {
            var assetBundle = Load();
            var loadedAsset = assetBundle.LoadAsset<TAsset>(name);
            EnsureAsset(loadedAsset, name, exception => throw exception);

            return loadedAsset;
        }

        public IDisposable LoadAsync<TAsset>(string name, IAssetLoadingCallbacks<TAsset> callbacks)
            where TAsset : Object
        {
            var loadingCoroutine = new AssetFileLoadingCoroutine(_coroutineHost);
            return LoadAsync(new AssetLoadingCallbacks<AssetBundle>(
                    onLoaded: bundle => loadingCoroutine.Start(LoadingProcess(name, bundle,
                        callbacks.AddGuard(asset => EnsureAsset(asset, name, callbacks.InvokeFailedEvent)))),
                    onFailed: exception => throw exception));
        }

        public AssetBundle Load()
        {
            if (IsLoaded)
            {
                return _loadedAssetBundle;
            }

            var loadedAssetBundle = LoadOverride();
            EnsureAssetBundle(loadedAssetBundle);
            return _loadedAssetBundle = loadedAssetBundle;
        }

        public IDisposable LoadAsync(IAssetLoadingCallbacks<AssetBundle> callbacks)
        {
            if (IsLoaded)
            {
                callbacks.InvokeProgressEvent(1f);
                callbacks.InvokeLoadedEvent(_loadedAssetBundle);
                return new EmptyDisposable();
            }

            var loadingCoroutine = new AssetFileLoadingCoroutine(_coroutineHost);
            loadingCoroutine.Start(LoadingProcess(callbacks
                .AddGuard(EnsureAssetBundle)
                .ReturnTo(asset => _loadedAssetBundle = asset)));

            return loadingCoroutine;
        }

        public void Unload()
        {
            if (IsLoaded == false)
            {
                return;
            }

            _loadedAssetBundle.Unload(true);
            _loadedAssetBundle = null;
        }

        public void UnloadAsync()
        {
            if (IsLoaded == false)
            {
                return;
            }

            _loadedAssetBundle.UnloadAsync(true);
            _loadedAssetBundle = null;
        }

        public IEnumerable<string> AllAssetNames() => Load().GetAllAssetNames();

        protected abstract AssetBundle LoadOverride();

        protected abstract IEnumerator LoadingProcess(IAssetLoadingCallbacks<AssetBundle> callbacks);

        private IEnumerator LoadingProcess<TAsset>(string name, AssetBundle bundle,
            IAssetLoadingCallbacks<TAsset> callbacks) where TAsset : Object
        {
            var request = bundle.LoadAssetAsync<TAsset>(name);
            while (request.isDone == false)
            {
                callbacks.InvokeProgressEvent(request.progress);
                yield return null;
            }

            callbacks.InvokeLoadedEvent((TAsset)request.asset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureAsset(Object asset, string assetName, Action<Exception> onFailed)
        {
            if (asset == null)
            {
                onFailed?.Invoke(new AssetBundleFileLoadingException(assetName, Path));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureAssetBundle(AssetBundle assetBundle)
        {
            if (assetBundle == null)
            {
                throw new AssetBundleLoadingException(Path);
            }
        }

        void IDisposable.Dispose() => Unload();
    }
}