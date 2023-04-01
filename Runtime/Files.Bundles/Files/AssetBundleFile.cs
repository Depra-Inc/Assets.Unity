using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Async.Operations;
using Depra.Assets.Runtime.Async.Tokens;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Files.Bundles.Exceptions;
using Depra.Assets.Runtime.Files.Bundles.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Files.Bundles.Files
{
    public abstract class AssetBundleFile : ILoadableAsset<AssetBundle>, IDisposable
    {
        private readonly AssetIdent _ident;

        private AssetBundle _loadedAssetBundle;

        protected AssetBundleFile(AssetIdent ident)
        {
            _ident = ident;
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
                return AlreadyLoadedAsset<AssetBundle>.Create(_loadedAssetBundle, onLoaded, onProgress);
            }

            var request = RequestAsync();
            request.Start(OnLoadedInternal, onProgress, onFailed);
            void OnLoadedInternal(AssetBundle loadedBundle) => OnLoaded(loadedBundle, onFailed, onLoaded);

            return new AsyncActionToken(request.Cancel);
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

        protected abstract IAsyncLoad<AssetBundle> RequestAsync();
        
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