using System;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Abstract.Loading
{
    public readonly struct GuardedAssetLoadingCallbacks : IAssetLoadingCallbacks
    {
        private readonly Action<Object> _onEnsure;
        private readonly Action<Object> _onLoaded;
        private readonly Action<float> _onProgress;
        private readonly Action<Exception> _onFailed;
        
        public GuardedAssetLoadingCallbacks(IAssetLoadingCallbacks callbacks, Action<Object> onEnsure = null)
        {
            _onLoaded = callbacks.InvokeLoadedEvent;
            _onProgress = callbacks.InvokeProgressEvent;
            _onFailed = callbacks.InvokeFailedEvent;
            _onEnsure = onEnsure;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvokeLoadedEvent(Object loadedAsset)
        {
            _onEnsure?.Invoke(loadedAsset);
            _onLoaded?.Invoke(loadedAsset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvokeProgressEvent(float progress) =>
            _onProgress?.Invoke(progress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvokeFailedEvent(Exception exception) =>
            _onFailed?.Invoke(exception);
    }

    public readonly struct GuardedAssetLoadingCallbacks<TAsset> : IAssetLoadingCallbacks<TAsset> where TAsset : Object
    {
        private readonly Action<TAsset> _onEnsure;
        private readonly Action<TAsset> _onLoaded;
        private readonly Action<float> _onProgress;
        private readonly Action<Exception> _onFailed;

        public GuardedAssetLoadingCallbacks(IAssetLoadingCallbacks<TAsset> callbacks, Action<TAsset> onEnsure = null)
        {
            _onLoaded = callbacks.InvokeLoadedEvent;
            _onProgress = callbacks.InvokeProgressEvent;
            _onFailed = callbacks.InvokeFailedEvent;
            _onEnsure = onEnsure;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvokeLoadedEvent(TAsset loadedAsset)
        {
            _onEnsure?.Invoke(loadedAsset);
            _onLoaded?.Invoke(loadedAsset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvokeProgressEvent(float progress) =>
            _onProgress?.Invoke(progress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvokeFailedEvent(Exception exception) =>
            _onFailed?.Invoke(exception);
    }
}