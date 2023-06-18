using System;

namespace Depra.Assets.Runtime.Files.Resource
{
    internal sealed class PathDoesNotContainResourcesFolderException : Exception
    {
        private const string MESSAGE = "The specified path does not contain the Resources folder.";
        
        public PathDoesNotContainResourcesFolderException() : base(MESSAGE) { }
    }
}