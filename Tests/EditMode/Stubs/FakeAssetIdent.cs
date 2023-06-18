using Depra.Assets.Runtime.Files.Idents;

namespace Depra.Assets.Tests.EditMode.Stubs
{
    internal sealed class FakeAssetIdent : IAssetIdent
    {
        public FakeAssetIdent(string name)
        {
            Uri = name;
            RelativeUri = name;
        }

        public string Uri { get; }
        public string RelativeUri { get; }
    }
}