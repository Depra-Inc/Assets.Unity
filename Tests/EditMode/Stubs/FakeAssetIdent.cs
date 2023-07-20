using Depra.Assets.Idents;

namespace Depra.Assets.Unity.Tests.EditMode.Stubs
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