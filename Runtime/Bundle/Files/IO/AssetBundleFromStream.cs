using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Common;
using Depra.Coroutines.Domain.Entities;
using UnityEngine;

namespace Depra.Assets.Runtime.Bundle.Files.IO
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

        protected override IEnumerator LoadingProcess(IAssetLoadingCallbacks<AssetBundle> callbacks)
        {
            using var stream = OpenStream();
            var createRequest = AssetBundle.LoadFromStreamAsync(stream);
            while (createRequest.isDone == false)
            {
                callbacks.InvokeProgressEvent(createRequest.progress);
                yield return null;
            }

            callbacks.InvokeLoadedEvent(createRequest.assetBundle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Stream OpenStream() => new FileStream(Path, FileMode.Open, FileAccess.Read);
    }
}