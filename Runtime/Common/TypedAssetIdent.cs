using System;

namespace Depra.Assets.Runtime.Common
{
    public readonly struct TypedAssetIdent
    {
        public readonly Type Type;
        private readonly AssetIdent _ident;

        public TypedAssetIdent(Type type, AssetIdent ident)
        {
            Type = type;
            _ident = ident;
        }

        public string Name => _ident.Name;
        public string Path => _ident.Path;
    }
}