using System;
using Depra.Assets.Runtime.Async.Tokens;

namespace Depra.Assets.Runtime.Files.Interfaces
{
    public interface ILoadableAsset<out TAsset> : IAssetFile
    {
        bool IsLoaded { get; }
        
        TAsset Load();

        void Unload();

        IAsyncToken LoadAsync(
            Action<TAsset> onLoaded,
            Action<float> onProgress = null,
            Action<Exception> onFailed = null);
    }
}