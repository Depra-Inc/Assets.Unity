using System;
using Depra.Assets.Runtime.Async.Tokens;

namespace Depra.Assets.Runtime.Files.Structs
{
    public readonly struct AlreadyLoadedAsset<TAsset>
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