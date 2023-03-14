using System;
using Depra.Assets.Runtime.Abstract.Loading;

namespace Depra.Assets.Runtime.Interfaces.Requests
{
    public interface IUntypedAssetRequest : IAssetRequest, IDisposable
    {
        void Send(IAssetLoadingCallbacks callbacks);
    }
}