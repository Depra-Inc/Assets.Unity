using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Async.Threads;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Assets.Runtime.Files.Structs;
using Depra.Coroutines.Domain.Entities;
using UnityEngine;

namespace Depra.Assets.Runtime.Files.Bundles.IO
{
    public sealed class AssetBundleFromStream : AssetBundleFile
    {
        private readonly ICoroutineHost _coroutineHost;
        private AssetBundleCreateRequest _createRequest;

        public AssetBundleFromStream(AssetIdent ident, ICoroutineHost coroutineHost = null) : base(ident) =>
            _coroutineHost = coroutineHost;

        protected override AssetBundle LoadOverride()
        {
            RequiredFile.Ensure(Path, exception => throw exception);
            using var fileStream = OpenStream();
            var loadedAssetBundle = AssetBundle.LoadFromStream(fileStream);

            return loadedAssetBundle;
        }

        protected override IAssetThread<AssetBundle> RequestAsync() =>
            new MainAssetThread<AssetBundle>(_coroutineHost, LoadingProcess, CancelRequest);

        private IEnumerator LoadingProcess(Action<AssetBundle> onLoaded, Action<DownloadProgress> onProgress = null,
            Action<Exception> onFailed = null)
        {
            RequiredFile.Ensure(Path, onFailed);
            using var stream = OpenStream();
            _createRequest = AssetBundle.LoadFromStreamAsync(stream);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Stream OpenStream() => new FileStream(Path, FileMode.Open, FileAccess.Read);
    }
}