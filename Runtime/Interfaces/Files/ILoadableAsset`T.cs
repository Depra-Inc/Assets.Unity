using Depra.Assets.Runtime.Abstract.Loading;
using UnityEngine;

namespace Depra.Assets.Runtime.Interfaces.Files
{
    public interface ILoadableAsset<out TAsset> : IAssetFile where TAsset : Object
    {
        TAsset Load();

        void Unload();

        void LoadAsync(IAssetLoadingCallbacks<TAsset> callbacks);
    }
}