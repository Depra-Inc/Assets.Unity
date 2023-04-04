using System;
using System.Collections;
using Depra.Coroutines.Domain.Entities;

namespace Depra.Assets.Runtime.Async.Threads
{
    internal sealed class MainAssetThread<TAsset> : IAssetThread<TAsset>
    {
        private readonly Action _onCancel;
        private readonly ICoroutineHost _coroutineHost;
        private readonly Func<Action<TAsset>, Action<float>, Action<Exception>, IEnumerator> _processFactory;

        private ICoroutine _coroutine;

        public MainAssetThread(ICoroutineHost coroutineHost,
            Func<Action<TAsset>, Action<float>, Action<Exception>, IEnumerator> processFactory,
            Action onCancel = null)
        {
            _onCancel = onCancel;
            _coroutineHost = coroutineHost ?? throw new ArgumentNullException(nameof(coroutineHost));
            _processFactory = processFactory ?? throw new ArgumentNullException(nameof(processFactory));
        }

        public bool IsCompleted => _coroutine == null || _coroutine.IsDone;

        public void Start(
            Action<TAsset> onLoaded,
            Action<float> onProgress = null,
            Action<Exception> onFailed = null)
        {
            onLoaded += Complete;
            _coroutine = _coroutineHost.StartCoroutine(_processFactory(onLoaded, onProgress, onFailed));
        }

        public void Cancel()
        {
            if (IsCompleted)
            {
                return;
            }

            _coroutineHost.StopCoroutine(_coroutine);
            _onCancel?.Invoke();
        }

        private void Complete(TAsset loadedAsset)
        {
            _coroutine = null;
        }
    }
}