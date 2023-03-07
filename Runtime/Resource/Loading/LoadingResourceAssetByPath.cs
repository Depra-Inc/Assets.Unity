using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Resource.Loading
{
    public sealed class LoadingResourceAssetByPath : ResourceAssetSyncLoadingStrategy
    {
        private readonly string _path;

        public LoadingResourceAssetByPath(string path) =>
            _path = path;

        public override Object Load(Type assetType) => 
            Resources.Load(_path, assetType);

        public override TAsset Load<TAsset>() =>
            Resources.Load<TAsset>(_path);
    }
}