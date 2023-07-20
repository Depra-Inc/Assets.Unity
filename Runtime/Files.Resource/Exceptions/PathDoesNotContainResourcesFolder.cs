using System;

namespace Depra.Assets.Unity.Runtime.Files.Resource.Exceptions
{
    internal sealed class PathDoesNotContainResourcesFolder : Exception
    {
        private const string MESSAGE = "The specified path does not contain the Resources folder.";
        
        public PathDoesNotContainResourcesFolder() : base(MESSAGE) { }
    }
}