using System;
using System.Reflection;
using Depra.Assets.Runtime.Exceptions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Factory
{
    public sealed class ScriptableObjectFactory : AssetFactory
    {
        public override Object CreateAsset(Type type, string directory, string assetName,
            string typeExtension = AssetTypes.Base)
        {
            Object asset = ScriptableObject.CreateInstance(type);
#if UNITY_EDITOR
            asset = AssetDatabaseFactory.Create(asset, directory, assetName, typeExtension);
#endif

            return asset;
        }

        public override TAsset CreateAsset<TAsset>(string directory, string assetName,
            string typeExtension = AssetTypes.Base)
        {
            var type = typeof(TAsset);
            var asset = CreateAsset(type, directory, assetName);
            var assetAsT = asset as TAsset;
            EnsureAsset(assetAsT, type);

            return assetAsT;
        }

        private static void EnsureAsset<TAsset>(TAsset asset, MemberInfo assetType)
        {
            if (asset == null)
            {
                throw new AssetCreationException(assetType, assetType.Name);
            }
        }
    }
}