using System;
using System.Collections;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Interfaces.Files;
using Depra.Assets.Runtime.Interfaces.Strategies;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Bundle.Files
{
    public sealed class AssetBundleFileSource<TAsset> : IAssetFileSource<TAsset> where TAsset : Object
    {
        private readonly AssetBundleFile _assetBundle;
        private readonly AssetFileLoadingCoroutine _loadingCoroutine;

        public AssetBundleFileSource(AssetBundleFile assetBundle)
        {
            _assetBundle = assetBundle;
        }

        public TAsset Load(IAssetFile assetFile) =>
            _assetBundle.Load().LoadAsset<TAsset>(assetFile.Name);

        public IDisposable LoadAsync(IAssetFile assetFile, IAssetLoadingCallbacks<TAsset> callbacks) =>
            _assetBundle.LoadAsync(new AssetLoadingCallbacks<AssetBundle>(
                onLoaded: bundle => _loadingCoroutine.Start(LoadingProcess(assetFile.Path, bundle, callbacks)),
                onFailed: exception => throw exception));

        private IEnumerator LoadingProcess(string path, AssetBundle bundle,
            IAssetLoadingCallbacks<TAsset> callbacks)
        {
            var request = bundle.LoadAssetAsync<TAsset>(path);
            while (request.isDone == false)
            {
                callbacks.InvokeProgressEvent(request.progress);
                yield return null;
            }

            callbacks.InvokeLoadedEvent((TAsset)request.asset);
        }
    }
}