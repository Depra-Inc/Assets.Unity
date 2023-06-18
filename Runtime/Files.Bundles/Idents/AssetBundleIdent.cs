using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Files.Idents;
using UnityEngine;

namespace Depra.Assets.Runtime.Files.Bundles.Idents
{
    public sealed class AssetBundleIdent : IAssetIdent
    {
        private const string EXTENSION = ".assetbundle";

        public AssetBundleIdent(string name, string directory = null)
        {
            Name = name;
            AbsoluteDirectoryPath = directory ?? Application.streamingAssetsPath;
            AbsolutePath = System.IO.Path.Combine(AbsoluteDirectoryPath, Name + Extension);
            RelativePath = System.IO.Path.GetRelativePath(Constants.DataPathByPlatform, AbsolutePath);
        }

        public string Name { get; }
        public string Extension => EXTENSION;

        public string RelativePath { get; }
        public string AbsolutePath { get; }

        public string AbsoluteDirectoryPath { get; }

        string IAssetIdent.Uri => AbsolutePath;
        string IAssetIdent.RelativeUri => RelativePath;
    }
}