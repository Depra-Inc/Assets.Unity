using System;
using System.Reflection;

namespace Depra.Assets.Runtime.Files.Database
{
    public class AssetCreationException : Exception
    {
        public AssetCreationException(MemberInfo assetType, string assetName) : base(
            $"Asset {assetName} with type {assetType.Name} can not be created!") { }
    }
}