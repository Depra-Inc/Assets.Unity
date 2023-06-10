namespace Depra.Assets.Runtime.Files.Idents
{
    public sealed class FileSystemAssetIdent : IAssetWIthExtension
    {
        public static FileSystemAssetIdent Empty => new(string.Empty);
        public static FileSystemAssetIdent Invalid => new(nameof(Invalid));

        public readonly string Directory;

        public FileSystemAssetIdent(string path)
        {
            Path = path;
            Extension = System.IO.Path.GetExtension(Path);
            Directory = System.IO.Path.GetDirectoryName(Path);
            Name = System.IO.Path.GetFileNameWithoutExtension(Path);
        }

        public FileSystemAssetIdent(string name, string directory, string extension = null)
        {
            Name = name;
            Directory = directory;
            Path = System.IO.Path.Combine(Directory, Name);
            Extension = extension ?? System.IO.Path.GetExtension(Path);
        }

        public string Key => Path;
        public string Name { get; }
        public string Path { get; }
        public string Extension { get; }
        
        public string NameWithExtension => Name + Extension;
        
        string IAssetIdent.Uri => Path;

        //new DirectoryInfo(System.IO.Path.Combine(Application.dataPath, ident.Directory))

        public string AbsolutePath => System.IO.Path.Combine(Directory, Name + Extension);
    }
}