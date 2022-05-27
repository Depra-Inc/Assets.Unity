using System;
using System.IO;
using Depra.Assets.Runtime.Loading.Abstract;
using Depra.Assets.Runtime.Loading.Abstract.Sync;
using Depra.Assets.Runtime.Loading.Abstract.Utils;
using Depra.Assets.Runtime.Loading.Bundle.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Loading.Bundle.Sync
{
    public sealed class AssetBundleLoader : AssetLoader
    {
        public override Object LoadAsset(Type assetType, string assetPath)
        {
            SplitAssetPath(assetPath, out var bundlePath, out var assetName);
            var assetBundle = LoadAssetBundle(bundlePath);
            var asset = assetBundle.LoadAsset(assetName);

            if (asset == null)
            {
                assetBundle.Unload(false);
                throw new AssetLoadingException(assetType, assetPath);
            }

            assetBundle.Unload(false);
            return asset;
        }

        public override T LoadAsset<T>(string assetPath)
        {
            SplitAssetPath(assetPath, out var bundlePath, out var assetName);
            var assetBundle = LoadAssetBundle(bundlePath);
            var assetAsT = assetBundle.LoadAsset<T>(assetName);

            if (assetAsT == null)
            {
                assetBundle.Unload(false);
                throw new AssetLoadingException(typeof(T), assetPath);
            }

            assetBundle.Unload(false);
            return assetAsT;
        }

        public override void UnloadAsset(Object asset)
        {
            if (asset is AssetBundle assetBundle)
            {
                assetBundle.Unload(true);
            }
        }

        private static AssetBundle LoadAssetBundle(string bundlePath)
        {
            var loadedAssetBundle = AssetBundle.LoadFromFile(bundlePath);

            if (loadedAssetBundle == null)
            {
                loadedAssetBundle.Unload(false);
                throw new AssetBundleLoadingException(bundlePath);
            }

            return loadedAssetBundle;
        }

        private static void SplitAssetPath(string assetPath, out string bundlePath, out string assetName)
        {
            bundlePath = Path.GetDirectoryName(assetPath);
            assetName = Path.GetFileNameWithoutExtension(assetPath);
        }
    }
}