using System;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Abstract.Loading
{
    public interface IAssetLoadingCallbacks : IAssetLoadingCallbacks<Object> { }

    public interface IAssetLoadingCallbacks<in TAsset>
    {
        void InvokeLoadedEvent(TAsset loadedAsset);

        void InvokeProgressEvent(float progress);

        void InvokeFailedEvent(Exception exception);
    }
}