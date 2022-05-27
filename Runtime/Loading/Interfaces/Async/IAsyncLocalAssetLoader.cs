using System;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Loading.Interfaces.Async
{
    [PublicAPI]
    public interface IAsyncLocalAssetLoader
    {
        void LoadLocalAssetAsync(Type assetType, string assetPath, Action<Object> onLoaded);

        void LoadLocalAssetAsync<T>(string assetPath, Action<T> onLoaded) where T : Object;
    }
}