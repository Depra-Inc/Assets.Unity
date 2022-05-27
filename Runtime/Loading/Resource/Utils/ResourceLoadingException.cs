using System;

namespace Depra.Assets.Runtime.Loading.Resource.Utils
{
    internal class ResourceLoadingException : Exception
    {
        public ResourceLoadingException(Type type) : base($"Failed to load resource of type: {type}")
        {
        }

        public ResourceLoadingException(string assetPath) : base($"Failed to load resource by path: {assetPath}")
        {
        }

        public ResourceLoadingException(Type type, string assetPath) : base(
            $"Failed to load resource of type {type.Name} by path: {assetPath}")
        {
        }
    }
}