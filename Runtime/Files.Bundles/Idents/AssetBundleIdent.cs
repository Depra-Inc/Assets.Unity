using Depra.Assets.Runtime.Files.Idents;
using UnityEngine;

namespace Depra.Assets.Runtime.Files.Bundles.Idents
{
    public sealed class AssetBundleIdent : IAssetIdent
    {
        private const string EXTENSION = ".assetbundle";

        private static string DataPath
        {
            get
            {
#if UNITY_EDITOR
                return Application.dataPath;
#else
                return Application.persistentDataPath;
#endif
            }
        }
        
        public AssetBundleIdent(string name, string directory = null)
        {
            Name = name;
            AbsoluteDirectoryPath = directory ?? Application.streamingAssetsPath;
            AbsolutePath = System.IO.Path.Combine(AbsoluteDirectoryPath, Name + Extension);
            RelativePath = System.IO.Path.GetRelativePath(DataPath, AbsolutePath);
        }

        public string Uri { get; }
        public string Name { get; }
        public string Extension => EXTENSION;
        public string AbsolutePath { get; }
        public string AbsoluteDirectoryPath { get; }
        public string RelativePath { get; }
    }
}