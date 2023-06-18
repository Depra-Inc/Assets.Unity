using System.IO;
using Depra.Assets.Runtime.Extensions;
using static Depra.Assets.Runtime.Common.Constants;

namespace Depra.Assets.Runtime.Files.Idents
{
    public sealed class FileSystemAssetIdent : IAssetIdent
    {
        private readonly FileInfo _fileSystemInfo;

        public static FileSystemAssetIdent Empty => new(string.Empty);
        public static FileSystemAssetIdent Invalid => new(nameof(Invalid));

        public FileSystemAssetIdent(string path)
        {
            _fileSystemInfo = new FileInfo(path);
            _fileSystemInfo.Directory.CreateIfNotExists();
            Name = _fileSystemInfo.Name.Replace(Extension, string.Empty);
        }

        public FileSystemAssetIdent(string directory, string nameWithExtension) : this(
            Path.Combine(directory, nameWithExtension)) { }

        public FileSystemAssetIdent(string name, string directory, string extension = null) : this(
            Path.Combine(directory, name + extension)) { }

        public string Name { get; }
        public string NameWithExtension => Name + Extension;
        public string Extension => _fileSystemInfo.Extension;
        public string AbsolutePath => _fileSystemInfo.FullName;

        public string RelativePath
        {
            get
            {
                var path = AbsolutePath[DataPathByPlatform.Length..];
                var unityRelativePath = ASSETS_FOLDER_NAME + path;
                return unityRelativePath;
            }
        }

        public string AbsoluteDirectoryPath => _fileSystemInfo.DirectoryName;
        public string RelativeDirectoryPath => _fileSystemInfo.Directory!.Name;

        internal void ThrowIfNotExists()
        {
            if (_fileSystemInfo.Exists == false)
            {
                throw new FileNotFoundException($"File not found at path: {AbsolutePath}");
            }
        }

        string IAssetIdent.Uri => AbsolutePath;
        string IAssetIdent.RelativeUri => RelativePath;
    }
}