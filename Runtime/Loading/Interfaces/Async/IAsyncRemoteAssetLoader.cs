using System;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Loading.Interfaces.Async
{
    [PublicAPI]
    public interface IAsyncRemoteAssetLoader
    {
        void LoadRemoteAssetAsync(Type assetType, string urlAddress, string assetName, Action<Object> onLoaded);

        void LoadRemoteAssetAsync<T>(string urlAddress, string assetName, Action<T> onLoaded) where T : Object;
    }
}