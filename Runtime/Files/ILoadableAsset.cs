using System;
using Depra.Assets.Runtime.Async.Tokens;

namespace Depra.Assets.Runtime.Files
{
    public interface ILoadableAsset<out TAsset> : IAssetFile
    {
        TAsset Load();

        void Unload();

        IAsyncToken LoadAsync(
            Action<TAsset> onLoaded,
            Action<float> onProgress = null,
            Action<Exception> onFailed = null);
    }
}