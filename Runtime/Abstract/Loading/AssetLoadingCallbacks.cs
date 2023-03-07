using System;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Abstract.Loading
{
    public readonly struct AssetLoadingCallbacks : IAssetLoadingCallbacks
    {
        internal readonly Action<Object> OnLoaded;
        internal readonly Action<float> OnProgress;
        internal readonly Action<Exception> OnFailed;

        public AssetLoadingCallbacks(Action<Object> onLoaded, Action<float> onProgress = null,
            Action<Exception> onFailed = null)
        {
            OnLoaded = onLoaded;
            OnProgress = onProgress;
            OnFailed = onFailed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvokeLoadedEvent(Object loadedAsset) =>
            OnLoaded?.Invoke(loadedAsset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvokeProgressEvent(float progress) =>
            OnProgress?.Invoke(progress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvokeFailedEvent(Exception exception) =>
            OnFailed?.Invoke(exception);
    }

    public readonly struct AssetLoadingCallbacks<TAsset> : IAssetLoadingCallbacks<TAsset> where TAsset : Object
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

        public static implicit operator AssetLoadingCallbacks(AssetLoadingCallbacks<TAsset> callbacks) => new(
            onLoaded: @object => callbacks._onLoaded?.Invoke((TAsset)@object),
            onProgress: callbacks._onProgress,
            onFailed: callbacks._onFailed);
        
        public static implicit operator AssetLoadingCallbacks<TAsset>(AssetLoadingCallbacks callback) => new(
                callback.InvokeLoadedEvent,
                callback.InvokeProgressEvent,
                callback.InvokeFailedEvent);
    }
}