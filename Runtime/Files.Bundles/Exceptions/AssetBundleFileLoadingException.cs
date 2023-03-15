using System;

namespace Depra.Assets.Runtime.Files.Bundles.Exceptions
{
    internal sealed class AssetBundleFileLoadingException : Exception
    {
        private const string MESSAGE_FORMAT = "Failed to load asset {0} from asset bundle {1}!";

        public AssetBundleFileLoadingException(string assetName, string assetBundlePath) :
            base(string.Format(MESSAGE_FORMAT, assetName, assetBundlePath)) { }
    }
}