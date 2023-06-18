namespace Depra.Assets.Runtime.Files.Idents
{
    public interface IAssetIdent
    {
        public string Uri { get; }
        
        public string RelativeUri { get; }
    }
}