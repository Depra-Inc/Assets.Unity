using System;
using System.Collections;
using System.IO;
using Depra.Assets.Runtime.Loading.Abstract;
using Depra.Assets.Runtime.Loading.Abstract.Async;
using Depra.Assets.Runtime.Loading.Bundle.Utils;
using Depra.Utils.Runtime.Coroutines;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Loading.Bundle.Async
{
    public sealed class AsyncLocalAssetBundleLoader : AsyncLocalAssetLoader
    {
        private readonly ICoroutineRunner _coroutineRunner;

        public override void LoadLocalAssetAsync(Type assetType, string assetPath, Action<Object> onLoaded)
        {
            var bundlePath = Path.GetDirectoryName(assetPath);
            _coroutineRunner.StartCoroutine(LoadLocalBundleCoroutine(bundlePath,
                assetBundle =>
                {
                    var assetName = Path.GetFileNameWithoutExtension(assetPath);
                    _coroutineRunner.StartCoroutine(AssetBundleLoadingHelper.LoadAssetFromBundleCoroutine(
                        assetBundle, assetType, assetName, onLoaded));
                }));
        }

        public override void LoadLocalAssetAsync<T>(string assetPath, Action<T> onLoaded)
        {
            var bundlePath = Path.GetDirectoryName(assetPath);
            _coroutineRunner.StartCoroutine(LoadLocalBundleCoroutine(bundlePath,
                assetBundle =>
                {
                    var assetName = Path.GetFileNameWithoutExtension(assetPath);
                    _coroutineRunner.StartCoroutine(AssetBundleLoadingHelper.LoadAssetFromBundleCoroutine(
                        assetBundle, assetName, onLoaded));
                }));
        }

        public AsyncLocalAssetBundleLoader([NotNull] ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
        }

        private static IEnumerator LoadLocalBundleCoroutine(string bundlePath, Action<AssetBundle> onLoaded)
        {
            var bundleLoadRequest = AssetBundle.LoadFromFileAsync(bundlePath);
            yield return bundleLoadRequest;

            var loadedAssetBundle = bundleLoadRequest.assetBundle;
            if (loadedAssetBundle == null)
            {
                throw new AssetBundleLoadingException(bundlePath);
            }

            onLoaded.Invoke(loadedAssetBundle);
        }
    }
}