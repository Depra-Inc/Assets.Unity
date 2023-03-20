using System;
using System.Collections;
using System.IO;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Coroutines.Domain.Entities;
using UnityEngine;

namespace Depra.Assets.Runtime.Files.Bundles.Memory
{
    public sealed class AssetBundleFromMemory : AssetBundleFile
    {
        private readonly byte[] _bytes;
        private readonly ICoroutineHost _coroutineHost;

        public AssetBundleFromMemory(AssetIdent ident, ICoroutineHost coroutineHost = null) :
            base(ident, coroutineHost) => _bytes = File.ReadAllBytes(Path);

        protected override AssetBundle LoadOverride() => AssetBundle.LoadFromMemory(_bytes);

        protected override IEnumerator LoadingProcess(Action<AssetBundle> onLoaded, Action<float> onProgress = null,
            Action<Exception> onFailed = null)
        {
            var createRequest = AssetBundle.LoadFromMemoryAsync(_bytes);
            while (createRequest.isDone == false)
            {
                onProgress?.Invoke(createRequest.progress);
                yield return null;
            }

            onProgress?.Invoke(1f);
            onLoaded.Invoke(createRequest.assetBundle);
        }
    }
}