using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Delegates;
using Depra.Assets.Files;
using Depra.Assets.Idents;
using Depra.Assets.Unity.Runtime.Files.Adapter;
using Depra.Assets.Unity.Tests.PlayMode.Stubs;
using Depra.Assets.ValueObjects;

namespace Depra.Assets.Unity.Tests.EditMode.Stubs
{
    internal sealed class FakeAssetFile : IUnityLoadableAsset<TestScriptableAsset>
    {
        public FakeAssetFile(IAssetIdent ident) => Ident = ident;

        public IAssetIdent Ident { get; }

        public bool IsLoaded { get; private set; }

        FileSize IAssetFile.Size => FileSize.Zero;

        TestScriptableAsset IUnityLoadableAsset<TestScriptableAsset>.Load() =>
            throw new NotImplementedException();

        void IUnityLoadableAsset<TestScriptableAsset>.Unload() => IsLoaded = false;

        UniTask<TestScriptableAsset> IUnityLoadableAsset<TestScriptableAsset>.LoadAsync(
            DownloadProgressDelegate onProgress,
            CancellationToken cancellationToken) =>
            throw new NotImplementedException();
    }
}