using System;
using System.Collections;
using System.IO;
using Depra.Assets.Runtime.Async.Threads;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Assets.Runtime.Files.Structs;
using Depra.Coroutines.Domain.Entities;
using UnityEngine;

namespace Depra.Assets.Runtime.Files.Bundles.Memory
{
    public sealed class AssetBundleFromMemory : AssetBundleFile
    {
        private readonly byte[] _bytes;
        private readonly ICoroutineHost _coroutineHost;
        private AssetBundleCreateRequest _createRequest;

        public AssetBundleFromMemory(AssetIdent ident, ICoroutineHost coroutineHost = null) :
            base(ident)
        {
            _bytes = File.ReadAllBytes(Path);
            _coroutineHost = coroutineHost;
        }

        protected override AssetBundle LoadOverride() => 
            AssetBundle.LoadFromMemory(_bytes);

        protected override IAssetThread<AssetBundle> RequestAsync() =>
            new MainAssetThread<AssetBundle>(_coroutineHost, LoadingProcess, CancelRequest);

        private IEnumerator LoadingProcess(Action<AssetBundle> onLoaded, Action<float> onProgress = null, Action<Exception> onFailed = null)
        {
            _createRequest = AssetBundle.LoadFromMemoryAsync(_bytes);
            while (_createRequest.isDone == false)
            {
                onProgress?.Invoke(_createRequest.progress);
                yield return null;
            }

            onProgress?.Invoke(1f);
            onLoaded.Invoke(_createRequest.assetBundle);
        }

        private void CancelRequest()
        {
            if (_createRequest == null || _createRequest.assetBundle == null)
            {
                return;
            }
            
            _createRequest.assetBundle.Unload(true);
            _createRequest = null;
        }
    }
}