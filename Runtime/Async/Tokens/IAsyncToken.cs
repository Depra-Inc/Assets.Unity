namespace Depra.Assets.Runtime.Async.Tokens
{
    public interface IAsyncToken
    {
        bool IsCanceled { get; }

        void Cancel();
    }
}