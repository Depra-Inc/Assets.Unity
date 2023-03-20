using System;
using System.Threading;
using System.Threading.Tasks;

namespace Depra.Assets.Runtime.Common
{
    public interface IAsyncToken
    {
        bool IsCompleted { get; }
        
        void Cancel();
    }
    
    public sealed class TaskToken : IAsyncToken
    {
        public bool IsCompleted { get; }
        public void Cancel()
        {
            throw new NotImplementedException();
        }
    }

    public sealed class AsyncActionToken : IAsyncToken, IDisposable
    {
        private readonly Action _onCancel;

        public static AsyncActionToken Empty { get; } = new(() => { });

        public static AsyncActionToken FromTask(CancellationToken cancellationToken, Task task)
        {
            var token = new AsyncActionToken(cancellationToken.ThrowIfCancellationRequested);
            if (task.IsCompleted)
            {
                token.Complete();
            }
            
            task.ContinueWith(_ => token.Complete(), cancellationToken);
            
            return token;
        }

        public AsyncActionToken(Action onCancel) => _onCancel = onCancel;

        public bool IsCompleted { get; private set; }

        public void Cancel()
        {
            if (IsCompleted == false)
            {
                _onCancel?.Invoke();
            }
        }

        internal void Complete() => IsCompleted = true;

        void IDisposable.Dispose() => Cancel();
    }
}