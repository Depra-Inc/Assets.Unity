using System;

namespace Depra.Assets.Runtime.Files.Exceptions
{
    internal sealed class AssetAlreadyAddedToGroup : Exception
    {
        private const string MESSAGE_FORMAT = "Asset {0} already added to group!";

        public AssetAlreadyAddedToGroup(string assetName) : base(string.Format(MESSAGE_FORMAT, assetName)) { }
    }
}