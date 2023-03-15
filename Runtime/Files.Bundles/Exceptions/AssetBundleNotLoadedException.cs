using System;

namespace Depra.Assets.Runtime.Files.Bundles.Exceptions
{
    internal sealed class AssetBundleNotLoadedException : Exception
    {
        private const string MESSAGE_FORMAT = "Asset bundle at path {0} was not loaded.";
        
        public AssetBundleNotLoadedException(string path) :
            base(string.Format(MESSAGE_FORMAT, path)) { }
    }
}