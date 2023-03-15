using System;
using Depra.Assets.Runtime.Abstract.Loading;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Files
{
    public interface ILoadableAsset<out TAsset> : IAssetFile where TAsset : Object
    {
        TAsset Load();

        void Unload();

        IDisposable LoadAsync(IAssetLoadingCallbacks<TAsset> callbacks);
    }
}