using System;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Interfaces.Files;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Interfaces.Strategies
{
    public interface IAssetFileSource<out TAsset> where TAsset : Object
    {
        TAsset Load(IAssetFile assetFile);

        IDisposable LoadAsync(IAssetFile assetFile, IAssetLoadingCallbacks<TAsset> callbacks);
    }
}