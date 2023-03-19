using System;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Abstract.Loading
{
    public readonly struct AssetLoadingCallbacks<TAsset> : IAssetLoadingCallbacks<TAsset>
    {
        private readonly Action<TAsset> _onLoaded;
        private readonly Action<float> _onProgress;
        private readonly Action<Exception> _onFailed;

        public AssetLoadingCallbacks(Action<TAsset> onLoaded, Action<float> onProgress = null,
            Action<Exception> onFailed = null)
        {
            _onLoaded = onLoaded;
            _onProgress = onProgress;
            _onFailed = onFailed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvokeLoadedEvent(TAsset loadedAsset) =>
            _onLoaded?.Invoke(loadedAsset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvokeProgressEvent(float progress) =>
            _onProgress?.Invoke(progress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvokeFailedEvent(Exception exception) =>
            _onFailed?.Invoke(exception);
    }
}