using System;
using Depra.Assets.Runtime.Loading.Interfaces.Async;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Loading.Abstract.Async
{
    public abstract class AsyncRemoteAssetLoader : IAsyncRemoteAssetLoader
    {
        public abstract void LoadRemoteAssetAsync(Type assetType, string urlAddress, string assetName,
            Action<Object> onLoaded);

        public abstract void LoadRemoteAssetAsync<T>(string urlAddress, string assetName, Action<T> onLoaded)
            where T : Object;
    }
}