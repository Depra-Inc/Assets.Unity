using System;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Services.Async
{
    public class AsyncAssetProvisionService : IAsyncAssetProvisionService
    {
        public void GetLocalAssetAsync(Type assetType, string directory, string assetName, Action<Object> onComplete)
        {
            throw new NotImplementedException();
        }

        public void GetLocalAssetAsync<T>(string directory, string assetName, Action<T> onComplete)
        {
            throw new NotImplementedException();
        }

        public void GetRemoteAssetAsync(Type assetType, string urlAddress, string assetName, Action<Object> onComplete)
        {
            throw new NotImplementedException();
        }

        public void GetRemoteAssetAsync<T>(string urlAddress, string assetName, Action<T> onComplete)
        {
            throw new NotImplementedException();
        }
    }
}