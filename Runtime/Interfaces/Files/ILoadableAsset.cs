using Depra.Assets.Runtime.Abstract.Loading;
using UnityEngine;

namespace Depra.Assets.Runtime.Interfaces.Files
{
    public interface ILoadableAsset : IAssetFile
    {
        Object Load();

        void Unload();

        void LoadAsync(IAssetLoadingCallbacks callbacks);
    }
}