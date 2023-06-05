using System;

namespace Depra.Assets.Runtime.Files.Exceptions
{
    internal sealed class AssetAlreadyLoadedException : Exception
    {
        private const string MESSAGE_FORMAT = "Asset {0} already loaded!";

        public AssetAlreadyLoadedException(string assetName) : base(string.Format(MESSAGE_FORMAT, assetName)) { }
    }
}