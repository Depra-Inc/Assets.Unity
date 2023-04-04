using Depra.Assets.Runtime.Files.Structs;

namespace Depra.Assets.Runtime.Files.Interfaces
{
    public interface IAssetFile
    {
        string Name { get; }
        
        string Path { get; }
        
        FileSize Size { get; }
    }
}