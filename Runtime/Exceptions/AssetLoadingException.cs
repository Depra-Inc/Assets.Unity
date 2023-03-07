using System;
using System.IO;
using System.Reflection;

namespace Depra.Assets.Runtime.Exceptions
{
    public class AssetLoadingException : Exception
    {
        public AssetLoadingException(MemberInfo type) : 
            base($"Fail to load asset of type [{type.Name}]") { }

        public AssetLoadingException(MemberInfo type, string assetPath) :
            base($"Fail to load asset of type [{type.Name}] by path: {assetPath}") { }

        public AssetLoadingException(MemberInfo type, string assetDirectory, string assetName) :
            this(type, Path.Combine(assetDirectory, assetName)) { }
    }
}