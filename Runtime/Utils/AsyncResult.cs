namespace Depra.Assets.Runtime.Utils
{
    public class AsyncResult<T>
    {
        public T Result { get; }

        public bool Loaded => Result != null;

        public AsyncResult(T result)
        {
            Result = result;
        }
    }
}