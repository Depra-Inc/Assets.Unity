using System;

namespace Depra.Assets.Runtime.Files
{
    public interface ILoadableAsset<out TAsset> : IAssetFile
    {
        TAsset Load();

        void Unload();

        IDisposable LoadAsync(
            Action<TAsset> onLoaded,
            Action<float> onProgress = null,
            Action<Exception> onFailed = null);
    }
}