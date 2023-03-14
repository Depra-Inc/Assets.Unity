using System;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Abstract.Loading
{
    public static class AssetLoadingCallbacksExtensions
    {
        public static IAssetLoadingCallbacks<TAsset> AddGuard<TAsset>(this IAssetLoadingCallbacks<TAsset> callbacks,
            Action<TAsset> guard) where TAsset : Object =>
            new GuardedAssetLoadingCallbacks<TAsset>(callbacks, guard);

        public static IAssetLoadingCallbacks<TAsset> ReturnTo<TAsset>(this IAssetLoadingCallbacks<TAsset> callbacks,
            Action<TAsset> onReady) where TAsset : Object =>
            new ReturnedAssetLoadingCallbacks<TAsset>(onReady, callbacks);
    }
}