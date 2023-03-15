using System;

namespace Depra.Assets.Runtime.Files.Bundles.Exceptions
{
    internal sealed class AssetBundleLoadingException : Exception
    {
        private const string MESSAGE_FORMAT = "Failed to load asset bundle by path: {0}";

        public AssetBundleLoadingException(string assetBundlePath) :
            base(string.Format(MESSAGE_FORMAT, assetBundlePath)) { }
    }
}