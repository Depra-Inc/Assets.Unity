using System;
using Depra.Assets.Runtime.Abstract.Loading;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Interfaces.Requests
{
    public interface ITypedAssetRequest<out TAsset> : IAssetRequest, IDisposable where TAsset : Object
    {
        void Send(IAssetLoadingCallbacks<TAsset> callbacks);
    }
}