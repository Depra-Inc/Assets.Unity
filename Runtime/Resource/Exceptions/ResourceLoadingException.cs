using System;
using System.Reflection;

namespace Depra.Assets.Runtime.Resource.Exceptions
{
    internal sealed class ResourceLoadingException : Exception
    {
        public ResourceLoadingException(MemberInfo type) : 
            base($"Failed to load resource of type: {type.Name}") { }

        public ResourceLoadingException(string assetPath) : 
            base($"Failed to load resource by path: {assetPath}") { }

        public ResourceLoadingException(MemberInfo type, string assetPath) :
            base($"Failed to load resource of type {type.Name} by path: {assetPath}") { }
    }
}