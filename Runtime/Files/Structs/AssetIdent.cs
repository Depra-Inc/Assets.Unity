namespace Depra.Assets.Runtime.Files.Structs
{
    public readonly struct AssetIdent
    {
        public readonly string Name;
        public readonly string Path;
        
        public AssetIdent(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileName(path);
        }

        public AssetIdent(string name, string directory)
        {
            Name = name;
            Path = System.IO.Path.Combine(directory, name);
        }
    }
}