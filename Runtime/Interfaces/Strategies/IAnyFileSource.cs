using Depra.Assets.Runtime.Abstract.Loading;
using UnityEngine;

namespace Depra.Assets.Runtime.Interfaces.Strategies
{
    public interface IAnyFileSource
    {
        TAsset Load<TAsset>(string path) where TAsset : Object;

        void LoadAsync<TAsset>(string path, IAssetLoadingCallbacks<TAsset> callbacks) where TAsset : Object;
    }
}