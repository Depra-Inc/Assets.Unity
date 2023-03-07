using System;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Resource.Loading
{
    public abstract class ResourceAssetSyncLoadingStrategy
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract Object Load(Type assetType);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract TAsset Load<TAsset>() where TAsset : Object;
    }
}