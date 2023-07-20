using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Delegates;
using Depra.Assets.Files;
using Depra.Assets.Idents;
using Depra.Assets.Unity.Runtime.Files.Adapter;
using Depra.Assets.ValueObjects;
using UnityEngine;

namespace Depra.Assets.Unity.Tests.PlayMode.Stubs
{
    internal sealed class FakeAssetFile : IUnityLoadableAsset<Object>
    {
        private static TestScriptableAsset CreateAsset() =>
            ScriptableObject.CreateInstance<TestScriptableAsset>();

        public bool IsLoaded { get; private set; }

        FileSize IAssetFile.Size { get; } = new(1);

        IAssetIdent IAssetFile.Ident => AssetName.Empty;

        Object IUnityLoadableAsset<Object>.Load()
        {
            IsLoaded = true;
            return CreateAsset();
        }

        async UniTask<Object> IUnityLoadableAsset<Object>.LoadAsync(DownloadProgressDelegate onProgress,
            CancellationToken cancellationToken)
        {
            var asset = CreateAsset();
            onProgress?.Invoke(DownloadProgress.Full);
            IsLoaded = true;

            await UniTask.Yield();

            return asset;
        }

        void IUnityLoadableAsset<Object>.Unload() => IsLoaded = false;
    }
}