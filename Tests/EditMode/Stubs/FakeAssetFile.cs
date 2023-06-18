using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Runtime.Files.Delegates;
using Depra.Assets.Runtime.Files.Idents;
using Depra.Assets.Runtime.Files.Interfaces;
using Depra.Assets.Runtime.Files.ValueObjects;
using Depra.Assets.Tests.PlayMode.Stubs;

namespace Depra.Assets.Tests.EditMode.Stubs
{
    internal sealed class FakeAssetFile : ILoadableAsset<TestScriptableAsset>
    {
        public FakeAssetFile(IAssetIdent ident) => Ident = ident;

        public IAssetIdent Ident { get; }

        public bool IsLoaded { get; private set; }

        FileSize IAssetFile.Size => FileSize.Zero;

        TestScriptableAsset ILoadableAsset<TestScriptableAsset>.Load() =>
            throw new NotImplementedException();

        void ILoadableAsset<TestScriptableAsset>.Unload() => IsLoaded = false;

        UniTask<TestScriptableAsset> ILoadableAsset<TestScriptableAsset>.LoadAsync(
            DownloadProgressDelegate onProgress,
            CancellationToken cancellationToken) =>
            throw new NotImplementedException();
    }
}