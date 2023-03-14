using System;

namespace Depra.Assets.Runtime.Internal.Patterns
{
    internal sealed class EmptyDisposable : IDisposable
    {
        public void Dispose() { }
    }
}