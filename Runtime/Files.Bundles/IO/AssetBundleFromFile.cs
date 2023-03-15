using System.Collections;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Bundle.Files;
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

        protected override IEnumerator LoadingProcess(IAssetLoadingCallbacks<AssetBundle> callbacks)
        {
            var createRequest = AssetBundle.LoadFromFileAsync(Path);
            while (createRequest.isDone == false)
            {
                callbacks.InvokeProgressEvent(createRequest.progress);
                yield return null;
            }

            callbacks.InvokeLoadedEvent(createRequest.assetBundle);
        }
    }
}