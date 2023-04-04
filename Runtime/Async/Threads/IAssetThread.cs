using System;

namespace Depra.Assets.Runtime.Async.Threads
{
    public interface IAssetThread<out TAsset>
    {
        void Start(Action<TAsset> onLoaded, Action<float> onProgress, Action<Exception> onFailed);

        void Cancel();
    }
}