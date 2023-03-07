using System;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Abstract.Loading
{
    public readonly struct ReturnedAssetLoadingCallbacks : IAssetLoadingCallbacks
    {
        private readonly Action<Object> _onReturn;
        private readonly Action<Object> _onLoaded;
        private readonly Action<float> _onProgress;
        private readonly Action<Exception> _onFailed;

        public ReturnedAssetLoadingCallbacks(Action<Object> onReturn, IAssetLoadingCallbacks<Object> callbacks)
        {
            _onReturn = onReturn;
            _onLoaded = callbacks.InvokeLoadedEvent;
            _onProgress = callbacks.InvokeProgressEvent;
            _onFailed = callbacks.InvokeFailedEvent;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvokeLoadedEvent(Object loadedAsset)
        {
            _onLoaded?.Invoke(loadedAsset);
            _onReturn?.Invoke(loadedAsset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvokeProgressEvent(float progress) =>
            _onProgress?.Invoke(progress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvokeFailedEvent(Exception exception) =>
            _onFailed?.Invoke(exception);
    }

    public readonly struct ReturnedAssetLoadingCallbacks<TAsset>: IAssetLoadingCallbacks<TAsset> where TAsset : Object
    {
        private readonly Action<TAsset> _onReturn;
        private readonly Action<TAsset> _onLoaded;
        private readonly Action<float> _onProgress;
        private readonly Action<Exception> _onFailed;

        public ReturnedAssetLoadingCallbacks(Action<TAsset> onReturn, IAssetLoadingCallbacks<TAsset> callbacks)
        {
            _onReturn = onReturn;
            _onLoaded = callbacks.InvokeLoadedEvent;
            _onProgress = callbacks.InvokeProgressEvent;
            _onFailed = callbacks.InvokeFailedEvent;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvokeLoadedEvent(TAsset loadedAsset)
        {
            _onLoaded?.Invoke(loadedAsset);
            _onReturn?.Invoke(loadedAsset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvokeProgressEvent(float progress) =>
            _onProgress?.Invoke(progress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvokeFailedEvent(Exception exception) =>
            _onFailed?.Invoke(exception);
    }
}