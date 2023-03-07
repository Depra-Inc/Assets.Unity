namespace Depra.Assets.Runtime.Interfaces.Files
{
    public interface IAssetFile
    {
        string Name { get; }
        
        string Path { get; }
        
        bool IsLoaded { get; }
    }
}