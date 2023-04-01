using System;
using Depra.Assets.Runtime.Async.Tokens;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Common
{
    internal struct AlreadyLoadedAsset<TAsset>
    {
        private const float MAX_PROGRESS = 1f;

        public static IAsyncToken Create(TAsset loadedAsset, Action<TAsset> onLoaded, Action<float> onProgress)
        {
            onProgress?.Invoke(MAX_PROGRESS);
            onLoaded.Invoke(loadedAsset);
                
            return AsyncActionToken.Empty;
        }
    }
}