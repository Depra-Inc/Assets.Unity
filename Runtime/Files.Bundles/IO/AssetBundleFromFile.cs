using System;
using System.Collections;
using Depra.Assets.Runtime.Async.Threads;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Assets.Runtime.Files.Structs;
using Depra.Assets.Runtime.Utils;
using Depra.Coroutines.Domain.Entities;
using UnityEngine;

namespace Depra.Assets.Runtime.Files.Bundles.IO
{
    public sealed class AssetBundleFromFile : AssetBundleFile
    {
        private readonly ICoroutineHost _coroutineHost;
        private AssetBundleCreateRequest _createRequest;

        public AssetBundleFromFile(AssetIdent ident, ICoroutineHost coroutineHost = null) : base(ident) =>
            _coroutineHost = coroutineHost ?? AssetCoroutineHook.Instance;

        protected override AssetBundle LoadOverride()
        {
            RequiredFile.Ensure(Path, exception => throw exception);
            var loadedBundle = AssetBundle.LoadFromFile(Path);
            
            return loadedBundle;
        }

        protected override IAssetThread<AssetBundle> RequestAsync() =>
            new MainAssetThread<AssetBundle>(_coroutineHost, LoadingProcess, CancelRequest);

        private IEnumerator LoadingProcess(
            Action<AssetBundle> onLoaded,
            Action<DownloadProgress> onProgress = null,
            Action<Exception> onFailed = null)
        {
            RequiredFile.Ensure(Path, onFailed);
            _createRequest = AssetBundle.LoadFromFileAsync(Path);
            while (_createRequest.isDone == false)
            {
                var progress = new DownloadProgress(_createRequest.progress);
                onProgress?.Invoke(progress);

                yield return null;
            }

            onProgress?.Invoke(DownloadProgress.Full);
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