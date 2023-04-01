using System;
using Depra.Assets.Runtime.Async.Tokens;

namespace Depra.Assets.Runtime.Async.Operations
{
    public interface IAsyncLoad<out TAsset>
    {
        void Start(Action<TAsset> onLoaded, Action<float> onProgress, Action<Exception> onFailed);

        void Cancel();
    }
}