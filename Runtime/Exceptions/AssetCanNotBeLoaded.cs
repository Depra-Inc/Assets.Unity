using System;

namespace Depra.Assets.Unity.Runtime.Exceptions
{
    internal sealed class AssetCanNotBeLoaded : Exception
    {
        public AssetCanNotBeLoaded(string message) : base(message) { }
    }
}