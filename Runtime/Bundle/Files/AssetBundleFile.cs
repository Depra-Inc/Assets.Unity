using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Bundle.Exceptions;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Interfaces.Files;
using Depra.Assets.Runtime.Interfaces.Strategies;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Bundle.Files
{
    public sealed class AssetBundleFile : ILoadableAsset, ILoadableAsset<AssetBundle>
    {
        private readonly AssetIdent _ident;
        private readonly AssetFileSource<AssetBundle> _source;

        private AssetBundle _loadedAssetBundle;

        public AssetBundleFile(AssetIdent ident, AssetFileSource<AssetBundle> source)
        {
            _ident = ident;
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public string Name => _ident.Name;
        public string Path => _ident.Path;
        public bool IsLoaded => _loadedAssetBundle != null;
        public IEnumerable<string> AllAssetNames => Load().GetAllAssetNames();

        public AssetBundle Load()
        {
            if (IsLoaded)
            {
                return _loadedAssetBundle;
            }

            var loadedAssetBundle = _source.Load(this);
            EnsureAssetBundle(loadedAssetBundle);
            return _loadedAssetBundle = loadedAssetBundle;
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

        public void LoadAsync(IAssetLoadingCallbacks callbacks) =>
            LoadAsync((IAssetLoadingCallbacks<AssetBundle>)callbacks);

        public void LoadAsync(IAssetLoadingCallbacks<AssetBundle> callbacks)
        {
            if (IsLoaded)
            {
                callbacks.InvokeProgressEvent(1f);
                callbacks.InvokeLoadedEvent(_loadedAssetBundle);
                return;
            }

            _source.LoadAsync(this, new ReturnedAssetLoadingCallbacks<AssetBundle>(
                bundle => _loadedAssetBundle = bundle,
                new GuardedAssetLoadingCallbacks<AssetBundle>(callbacks, EnsureAssetBundle)));
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureAssetBundle(AssetBundle assetBundle)
        {
            if (assetBundle == null)
            {
                throw new AssetBundleLoadingException(Path);
            }
        }

        Object ILoadableAsset.Load() => Load();
    }
}