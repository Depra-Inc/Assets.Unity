using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Runtime.Files.Interfaces;
using Depra.Assets.Runtime.Files.Resource;
using Depra.Assets.Runtime.Files.Structs;
using Depra.Assets.Tests.PlayMode.Types;
using UnityEngine;

namespace Depra.Assets.Tests.PlayMode.Mocks
{
    internal sealed class FakeAsset : ILoadableAsset<Object>
    {
        private static TestScriptableAsset CreateAsset() =>
            ScriptableObject.CreateInstance<TestScriptableAsset>();

        public FakeAsset()
        {
            Size = new FileSize(1);
            Name = nameof(FakeAsset);
            Path = Name;
        }

        public string Name { get; }

        public string Path { get; }

        public FileSize Size { get; }

        public bool IsLoaded { get; private set; }

        Object ILoadableAsset<Object>.Load()
        {
            IsLoaded = true;
            return CreateAsset();
        }

        async UniTask<Object> ILoadableAsset<Object>.LoadAsync(CancellationToken cancellationToken,
            DownloadProgressDelegate onProgress)
        {
            var asset = CreateAsset();
            onProgress?.Invoke(DownloadProgress.Full);
            IsLoaded = true;

            await UniTask.Yield();

            return asset;
        }

        void ILoadableAsset<Object>.Unload() => IsLoaded = false;
    }
}