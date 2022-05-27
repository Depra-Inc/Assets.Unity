using System;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Factory
{
    public abstract class AssetFactory : IAssetFactory
    {
        public abstract Object CreateAsset(Type type, string directory, string assetName,
            string typeExtension = AssetTypes.Base);

        public abstract T CreateAsset<T>(string directory, string assetName, string typeExtension = AssetTypes.Base)
            where T : Object;

        public virtual void DestroyAsset(Object asset)
        {
#if UNITY_EDITOR
            AssetDatabaseFactory.DeleteAsset(asset);
#endif
        }

        protected class AssetCreationException : Exception
        {
            public AssetCreationException(Type assetType, string assetName) : base(
                $"Asset {assetName} with type {assetType.Name} can not be created!")
            {
            }
        }
    }
}