using System;

namespace Depra.Assets.Runtime.Loading.Bundle.Utils
{
    [Serializable]
    internal class AssetBundleLoadingException : Exception
    {
        public AssetBundleLoadingException(string assetBundlePath) : base(
            $"Failed to load assetBundle by path: {assetBundlePath}")
        {
        }
    }
}