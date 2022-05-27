using System;

namespace Depra.Assets.Runtime.Loading.Abstract.Utils
{
    public class AssetLoadingException : Exception
    {
        public AssetLoadingException(Type type) : base($"Fail to load asset of type {type.Name}")
        {
        }

        public AssetLoadingException(Type type, string assetPath) : base(
            $"Fail to load asset of type {type.Name} by path: {assetPath}")
        {
        }
    }
}