using System;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Factory
{
    public abstract class AssetFactory
    {
        public abstract Object CreateAsset(Type type, string directory, string assetName,
            string typeExtension = AssetTypes.Base);

        public abstract T CreateAsset<T>(string directory, string assetName, string typeExtension = AssetTypes.Base)
            where T : Object;

        public void DestroyAsset(Object asset)
        {
#if UNITY_EDITOR
            AssetDatabaseFactory.Delete(asset);
#endif
        }

        public void DestroyAsset(string assetPath)
        {
#if UNITY_EDITOR
            AssetDatabaseFactory.Delete(assetPath);
#endif
        }
    }
}