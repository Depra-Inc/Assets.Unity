namespace Depra.Assets.Runtime.Files
{
    public interface IAssetFile
    {
        string Name { get; }
        
        string Path { get; }
        
        bool IsLoaded { get; }
    }
}