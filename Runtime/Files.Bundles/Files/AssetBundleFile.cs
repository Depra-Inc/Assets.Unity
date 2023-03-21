using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Async.Tokens;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Files.Bundles.Exceptions;
using Depra.Assets.Runtime.Files.Bundles.Extensions;
using Depra.Assets.Runtime.Utils;
using Depra.Coroutines.Domain.Entities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Files.Bundles.Files
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
        public FileSize Size { get; private set; } = FileSize.Unknown;

        public AssetBundle Load()
        {
            if (IsLoaded)
            {
                return _loadedAssetBundle;
            }

            var loadedAssetBundle = LoadOverride();
            return OnLoaded(loadedAssetBundle, onFailed: exception => throw exception);
        }

        public IAsyncToken LoadAsync(Action<AssetBundle> onLoaded, Action<float> onProgress = null,
            Action<Exception> onFailed = null)
        {
            if (IsLoaded)
            {
                onProgress?.Invoke(1f);
                onLoaded.Invoke(_loadedAssetBundle);
                
                return AsyncActionToken.Empty;
            }

            var loadingCoroutine = new AssetFileLoadingCoroutine(_coroutineHost);
            var asyncToken = new AsyncActionToken(loadingCoroutine.Cancel);
            onLoaded += _ => asyncToken.Complete();
            loadingCoroutine.Start(LoadingProcess(
                onLoaded: asset => OnLoaded(asset, onFailed, onLoaded),
                onProgress: onProgress));

            return asyncToken;
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

        protected abstract IEnumerator LoadingProcess(Action<AssetBundle> onLoaded, Action<float> onProgress = null,
            Action<Exception> onFailed = null);

        private AssetBundle OnLoaded(AssetBundle loadedBundle, Action<Exception> onFailed,
            Action<AssetBundle> onLoaded = null)
        {
            Ensure(loadedBundle, onFailed);
            _loadedAssetBundle = loadedBundle;
            onLoaded?.Invoke(loadedBundle);
            RefreshSize(_loadedAssetBundle);

            return _loadedAssetBundle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RefreshSize(AssetBundle assetBundle) =>
            Size = assetBundle.Size();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Ensure(Object assetBundle, Action<Exception> onFailed = null)
        {
            if (assetBundle == null)
            {
                onFailed?.Invoke(new AssetBundleNotLoadedException(Path));
            }
        }

        void IDisposable.Dispose() => Unload();
    }
}