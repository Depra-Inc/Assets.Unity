using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Loading.Bundle.Utils
{
    internal static class AssetBundleLoadingHelper
    {
        public static IEnumerator LoadAssetFromBundleCoroutine(AssetBundle assetBundle, Type assetType,
            string assetName,
            Action<Object> onLoaded = null)
        {
            var assetLoadRequest = assetBundle.LoadAssetAsync(assetName, assetType);
            yield return assetLoadRequest;

            onLoaded?.Invoke(assetLoadRequest.asset);

            assetBundle.Unload(false);
        }

        public static IEnumerator LoadAssetFromBundleCoroutine<T>(AssetBundle assetBundle, string assetName,
            Action<T> onLoaded = null) where T : Object
        {
            var assetLoadRequest = assetBundle.LoadAssetAsync<T>(assetName);
            yield return assetLoadRequest;

            var assetAsT = assetLoadRequest.asset as T;
            onLoaded?.Invoke(assetAsT);

            assetBundle.Unload(false);
        }
    }
}