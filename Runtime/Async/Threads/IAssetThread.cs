using System;
using Depra.Assets.Runtime.Files.Structs;

namespace Depra.Assets.Runtime.Async.Threads
{
    public interface IAssetThread<out TAsset>
    {
        void Start(Action<TAsset> onLoaded, Action<DownloadProgress> onProgress, Action<Exception> onFailed);

        void Cancel();
    }
}