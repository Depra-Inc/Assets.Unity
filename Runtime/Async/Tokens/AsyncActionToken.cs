using System;

namespace Depra.Assets.Runtime.Async.Tokens
{
    public sealed class AsyncActionToken : IAsyncToken, IDisposable
    {
        private readonly Action _onCancel;

        public static AsyncActionToken Empty { get; } = new(() => { });

        public AsyncActionToken(Action onCancel) => _onCancel = onCancel;

        public bool IsCanceled { get; private set; }

        public void Cancel()
        {
            if (IsCanceled)
            {
                throw new OperationCanceledException();
            }
            
            IsCanceled = true;
            _onCancel?.Invoke();
        }
        
        void IDisposable.Dispose() => Cancel();
    }
}