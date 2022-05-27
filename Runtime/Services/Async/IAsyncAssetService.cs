using System;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Services.Async
{
    [PublicAPI]
    public interface IAsyncAssetService
    {
        void GetLocalAssetAsync(Type assetType, string directory, string assetName, Action<Object> onComplete);

        void GetLocalAssetAsync<T>(string directory, string assetName, Action<T> onComplete);

        void GetRemoteAssetAsync(Type assetType, string urlAddress, string assetName, Action<Object> onComplete);

        void GetRemoteAssetAsync<T>(string urlAddress, string assetName, Action<T> onComplete);
    }
}