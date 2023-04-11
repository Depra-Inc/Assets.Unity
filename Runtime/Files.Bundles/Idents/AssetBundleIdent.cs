using System.IO;
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
            AbsolutePath = Path.Combine(AbsoluteDirectoryPath, Name + Extension);
            RelativePath = Path.GetRelativePath(DataPath, AbsolutePath);
        }
        
        public string Name { get; }
        public string Extension => EXTENSION;
        public string AbsolutePath { get; }
        public string AbsoluteDirectoryPath { get; }
        public string RelativePath { get; }
    }
}