using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Resource.Loading
{
    public sealed class LoadingAnyResourceAsset : ResourceAssetSyncLoadingStrategy
    {
        public override Object Load(Type assetType) => Resources
            .FindObjectsOfTypeAll(assetType)
            .FirstOrDefault();

        public override TAsset Load<TAsset>() => Resources
            .FindObjectsOfTypeAll<TAsset>()
            .FirstOrDefault();
    }
}