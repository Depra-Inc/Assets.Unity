using System;

namespace Depra.Assets.Runtime.Files.Bundles.Exceptions
{
    internal sealed class AssetBundleLoadingException : Exception
    {
        private const string MESSAGE_FORMAT = "Fail to load asset bundle {0} by path: {1}!";

        public AssetBundleLoadingException(string bundleName, string bundlePath) : base(
            string.Format(MESSAGE_FORMAT, bundleName, bundlePath)) { }
    }
}