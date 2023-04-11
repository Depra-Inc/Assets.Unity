using System;

namespace Depra.Assets.Runtime.Files.Bundles.Web
{
    internal sealed class RemoveAssetBundleNotLoadedException : Exception
    {
        private const string MESSAGE_FORMAT = "Error request [{0}, {1}]";

        public RemoveAssetBundleNotLoadedException(string url, string error) :
            base(string.Format(MESSAGE_FORMAT, url, error)) { }
    }
}