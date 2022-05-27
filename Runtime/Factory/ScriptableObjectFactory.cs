using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Factory
{
    public class ScriptableObjectFactory : AssetFactory
    {
        public override Object CreateAsset(Type type, string directory, string assetName,
            string typeExtension = AssetTypes.Base)
        {
            Object asset = ScriptableObject.CreateInstance(type);
#if UNITY_EDITOR
            asset = AssetDatabaseFactory.CreateAsset(asset, directory, assetName, typeExtension);
#endif

            return asset;
        }

        public override T CreateAsset<T>(string directory, string assetName, string typeExtension = AssetTypes.Base)
        {
            var type = typeof(T);
            var asset = CreateAsset(type, directory, assetName);
            var assetAsT = asset as T;

            if (assetAsT == null)
            {
                throw new AssetCreationException(type, type.Name);
            }

            return assetAsT;
        }
    }
}