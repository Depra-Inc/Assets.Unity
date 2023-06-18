using System.IO;
using Depra.Assets.Runtime.Files.Idents;
using static Depra.Assets.Runtime.Common.Constants;

namespace Depra.Assets.Runtime.Files.Database
{
    public readonly struct DatabaseAssetIdent : IAssetIdent
    {
        public DatabaseAssetIdent(string relativeDirectory, string name, string extension)
        {
            Name = name;
            Extension = extension;
            NameWithExtension = Name + Extension;
            AbsoluteDirectoryPath = Path.Combine(DataPathByPlatform, relativeDirectory);
            AbsoluteDirectory = new DirectoryInfo(AbsoluteDirectoryPath);
            AbsolutePath = Path.Combine(AbsoluteDirectoryPath, NameWithExtension);
            RelativePath = Path.Combine(ASSETS_FOLDER_NAME, relativeDirectory, NameWithExtension);
        }

        public string Name { get; }
        public string Extension { get; }
        public string NameWithExtension { get; }

        public string RelativePath { get; }
        public string AbsolutePath { get; }
        public string AbsoluteDirectoryPath { get; }
        public DirectoryInfo AbsoluteDirectory { get; }

        string IAssetIdent.Uri => AbsolutePath;
        string IAssetIdent.RelativeUri => RelativePath;
    }
}