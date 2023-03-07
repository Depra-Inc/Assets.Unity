using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Interfaces.Files;
using Depra.Assets.Runtime.Interfaces.Requests;
using UnityEngine;

namespace Depra.Assets.Runtime.Interfaces.Strategies
{
    public abstract class AssetFileSource<TAsset> where TAsset : Object
    {
        public abstract TAsset Load(IAssetFile assetFile);

        public void LoadAsync(IAssetFile assetFile, IAssetLoadingCallbacks<TAsset> callbacks)
        {
            using var request = CreateRequest(assetFile);
            request.Send(callbacks);
        }

        protected abstract ITypedAssetRequest<TAsset> CreateRequest(IAssetFile assetFile);
    }
}