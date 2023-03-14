using System;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Exceptions;
using Depra.Assets.Runtime.Interfaces.Files;
using Depra.Assets.Runtime.Internal.Patterns;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Bundle.Files
{
    public sealed class AssetBundleAssetFile<TAsset> : ILoadableAsset<TAsset>, IDisposable where TAsset : Object
    {
        private readonly AssetIdent _ident;
        private readonly AssetBundleFile _assetBundle;
        
        private TAsset _loadedAsset;

        public AssetBundleAssetFile(AssetIdent ident, AssetBundleFile assetBundle)
        {
            _ident = ident;
            _assetBundle = assetBundle ?? throw new ArgumentNullException(nameof(assetBundle));
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

            var assetAsT = _assetBundle.Load<TAsset>(Name);
            EnsureAsset(assetAsT);

            return assetAsT;
        }

        public void Unload()
        {
            if (IsLoaded)
            {
                _loadedAsset = null;
            }
        }

        public IDisposable LoadAsync(IAssetLoadingCallbacks<TAsset> callbacks)
        {
            if (IsLoaded)
            {
                callbacks.InvokeProgressEvent(1f);
                callbacks.InvokeLoadedEvent(_loadedAsset);
                return new EmptyDisposable();
            }

            return _assetBundle.LoadAsync(Name, callbacks
                .AddGuard(EnsureAsset)
                .ReturnTo(asset => _loadedAsset = asset));
        }

        public void Dispose() => Unload();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureAsset(TAsset asset)
        {
            if (asset == null)
            {
                throw new AssetLoadingException(typeof(TAsset), Path);
            }
        }

        public static implicit operator TAsset(AssetBundleAssetFile<TAsset> assetFile) => assetFile.Load();
    }
}