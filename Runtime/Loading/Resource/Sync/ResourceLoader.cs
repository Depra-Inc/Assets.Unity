using System;
using System.Linq;
using Depra.Assets.Runtime.Loading.Abstract;
using Depra.Assets.Runtime.Loading.Abstract.Sync;
using Depra.Assets.Runtime.Loading.Resource.Utils;
using Depra.Utils.Runtime.Coroutines;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Loading.Resource.Sync
{
    public sealed class ResourceLoader : AssetLoader
    {
        private readonly ICoroutineRunner _coroutineRunner;

        public override Object LoadAsset(Type assetType, string assetPath)
        {
            var asset = Resources.Load(assetPath);
            if (asset == null)
            {
                throw new ResourceLoadingException(assetType, assetPath);
            }

            return asset;
        }

        public override T LoadAsset<T>(string assetPath)
        {
            var assetAsT = Resources.Load<T>(assetPath);
            if (assetAsT == null)
            {
                throw new ResourceLoadingException(typeof(T), assetPath);
            }

            return assetAsT;
        }

        public override void UnloadAsset(Object asset)
        {
            Resources.UnloadAsset(asset);
        }

        protected override Object LoadAsset(Type assetType)
        {
            var asset = Resources.FindObjectsOfTypeAll(assetType).FirstOrDefault();
            if (asset == null)
            {
                throw new ResourceLoadingException(assetType);
            }

            return asset;
        }
    }
}