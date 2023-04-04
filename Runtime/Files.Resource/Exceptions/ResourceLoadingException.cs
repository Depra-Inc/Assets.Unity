using System;

namespace Depra.Assets.Runtime.Files.Resource.Exceptions
{
    internal sealed class ResourceLoadingException : Exception
    {
        private const string MESSAGE_FORMAT = "Failed to load resource {0} by path: {1}!";

        public ResourceLoadingException(string assetName, string assetPath) :
            base(string.Format(assetName, assetPath)) { }
    }
}