using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Async.Operations;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Files.Bundles.Files;
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
            using var fileStream = OpenStream();
            var loadedAssetBundle = AssetBundle.LoadFromStream(fileStream);

            return loadedAssetBundle;
        }

        protected override IAsyncLoad<AssetBundle> RequestAsync() =>
            new LoadFromMainThread<AssetBundle>(_coroutineHost, LoadingProcess, CancelRequest);

        private IEnumerator LoadingProcess(Action<AssetBundle> onLoaded, Action<float> onProgress = null,
            Action<Exception> onFailed = null)
        {
            using var stream = OpenStream();
            _createRequest = AssetBundle.LoadFromStreamAsync(stream);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Stream OpenStream() => new FileStream(Path, FileMode.Open, FileAccess.Read);
    }
}