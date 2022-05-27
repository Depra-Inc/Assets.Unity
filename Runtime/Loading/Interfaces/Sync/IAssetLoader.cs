using System;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Loading.Interfaces.Sync
{
    [PublicAPI]
    public interface IAssetLoader
    {
        Object LoadAsset(Type assetType, string assetPath);

        T LoadAsset<T>(string assetPath) where T : Object;

        void UnloadAsset(Object asset);
    }
}