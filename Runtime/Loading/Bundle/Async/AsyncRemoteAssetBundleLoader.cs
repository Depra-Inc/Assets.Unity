using System;
using System.Collections;
using Depra.Assets.Runtime.Loading.Abstract;
using Depra.Assets.Runtime.Loading.Abstract.Async;
using Depra.Assets.Runtime.Loading.Bundle.Utils;
using Depra.Utils.Runtime.Coroutines;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Loading.Bundle.Async
{
    public sealed class AsyncRemoteAssetBundleLoader : AsyncRemoteAssetLoader
    {
        private readonly ICoroutineRunner _coroutineRunner;

        public override void LoadRemoteAssetAsync(Type assetType, string urlAddress, string assetName,
            Action<Object> onLoaded)
        {
            _coroutineRunner.StartCoroutine(LoadRemoteAssetBundleCoroutine(urlAddress,
                bundle =>
                {
                    _coroutineRunner.StartCoroutine(AssetBundleLoadingHelper.LoadAssetFromBundleCoroutine(
                        bundle, assetType, assetName, onLoaded));
                }));
        }

        public override void LoadRemoteAssetAsync<T>(string urlAddress, string assetName, Action<T> onLoaded)
        {
            _coroutineRunner.StartCoroutine(LoadRemoteAssetBundleCoroutine(urlAddress, bundle =>
            {
                _coroutineRunner.StartCoroutine(AssetBundleLoadingHelper.LoadAssetFromBundleCoroutine(
                    bundle, assetName, onLoaded));
            }));
        }

        public AsyncRemoteAssetBundleLoader([NotNull] ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
        }

        private static IEnumerator LoadRemoteAssetBundleCoroutine(string urlAddress, Action<AssetBundle> onLoaded)
        {
            using (var request = UnityWebRequestAssetBundle.GetAssetBundle("https://www.hiperverso.com.br/AssetBundle"))
            {
                yield return request.SendWebRequest();

                if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
                {
                    throw new AssetBundleLoadingException(urlAddress);
                }

                var bundle = DownloadHandlerAssetBundle.GetContent(request);
                onLoaded.Invoke(bundle);
            }
        }
    }
}