using System;

namespace Depra.Assets.Runtime.Internal.Patterns
{
    public sealed class AsyncToken : IDisposable
    {
        private readonly Action _onCancel;

        public AsyncToken(Action onCancel)
        {
            _onCancel = onCancel;
        }

        public bool IsCompleted { get; private set; }

        public void Cancel()
        {
            if (IsCompleted == false)
            {
                _onCancel?.Invoke();
            }
        }

        void IDisposable.Dispose() => Cancel();
    }
}