using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Coroutines.Domain.Entities;
using UnityEngine;

namespace Depra.Assets.Runtime.Files.Bundles.IO
{
    public sealed class AssetBundleFromStream : AssetBundleFile
    {
        private readonly ICoroutineHost _coroutineHost;

        public AssetBundleFromStream(AssetIdent ident, ICoroutineHost coroutineHost = null) :
            base(ident, coroutineHost) { }

        protected override AssetBundle LoadOverride()
        {
            using var fileStream = OpenStream();
            var loadedAssetBundle = AssetBundle.LoadFromStream(fileStream);

            return loadedAssetBundle;
        }

        protected override IEnumerator LoadingProcess(Action<AssetBundle> onLoaded, Action<float> onProgress = null,
            Action<Exception> onFailed = null)
        {
            using var stream = OpenStream();
            var createRequest = AssetBundle.LoadFromStreamAsync(stream);
            while (createRequest.isDone == false)
            {
                onProgress?.Invoke(createRequest.progress);
                yield return null;
            }

            onProgress?.Invoke(1f);
            onLoaded.Invoke(createRequest.assetBundle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Stream OpenStream() => new FileStream(Path, FileMode.Open, FileAccess.Read);
    }
}