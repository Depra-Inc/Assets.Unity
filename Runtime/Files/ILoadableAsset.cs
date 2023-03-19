using System;
using Depra.Assets.Runtime.Abstract.Loading;

namespace Depra.Assets.Runtime.Files
{
    public interface ILoadableAsset<out TAsset> : IAssetFile
    {
        TAsset Load();

        void Unload();

        IDisposable LoadAsync(IAssetLoadingCallbacks<TAsset> callbacks);
    }
}