using System;
using System.Collections;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Coroutines.Domain.Entities;
using UnityEngine;

namespace Depra.Assets.Runtime.Files.Bundles.IO
{
    public sealed class AssetBundleFromFile : AssetBundleFile
    {
        private readonly ICoroutineHost _coroutineHost;

        public AssetBundleFromFile(AssetIdent ident, ICoroutineHost coroutineHost = null) :
            base(ident, coroutineHost) { }

        protected override AssetBundle LoadOverride() =>
            AssetBundle.LoadFromFile(Path);

        protected override IEnumerator LoadingProcess(Action<AssetBundle> onLoaded, Action<float> onProgress = null,
            Action<Exception> onFailed = null)
        {
            var createRequest = AssetBundle.LoadFromFileAsync(Path);
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