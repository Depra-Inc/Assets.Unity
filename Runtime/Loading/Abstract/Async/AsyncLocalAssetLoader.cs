using System;
using Depra.Assets.Runtime.Loading.Interfaces.Async;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Loading.Abstract.Async
{
    public abstract class AsyncLocalAssetLoader : IAsyncLocalAssetLoader
    {
        public abstract void LoadLocalAssetAsync(Type assetType, string assetPath, Action<Object> onLoaded);

        public abstract void LoadLocalAssetAsync<T>(string assetPath, Action<T> onLoaded) where T : Object;
    }
}