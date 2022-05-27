using System;
using System.Collections;
using Depra.Assets.Runtime.Loading.Abstract;
using Depra.Assets.Runtime.Loading.Abstract.Async;
using Depra.Assets.Runtime.Loading.Resource.Utils;
using Depra.Utils.Runtime.Coroutines;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Loading.Resource.Async
{
    public sealed class AsyncResourceLoader : AsyncLocalAssetLoader
    {
        private readonly ICoroutineRunner _coroutineRunner;

        public override void LoadLocalAssetAsync(Type assetType, string assetPath, Action<Object> onLoaded)
        {
            _coroutineRunner.StartCoroutine(LoadAssetCoroutine(assetType, assetPath, onLoaded));
        }

        public override void LoadLocalAssetAsync<T>(string assetPath, Action<T> onLoaded)
        {
            _coroutineRunner.StartCoroutine(LoadAssetCoroutine(assetPath, onLoaded));
        }

        public AsyncResourceLoader(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
        }

        private static IEnumerator LoadAssetCoroutine(Type assetType, string assetPath, Action<Object> onLoaded)
        {
            var request = Resources.LoadAsync(assetPath, assetType);
            while (request.isDone == false)
            {
                yield return null;
            }

            if (request.asset == null)
            {
                throw new ResourceLoadingException(assetType, assetPath);
            }

            onLoaded.Invoke(request.asset);
        }

        private static IEnumerator LoadAssetCoroutine<T>(string assetPath, Action<T> onLoaded) where T : Object
        {
            yield return LoadAssetCoroutine(typeof(T), assetPath, asset =>
            {
                var assetAsT = asset as T;
                if (assetAsT == null)
                {
                    throw new ResourceLoadingException(assetPath);
                }

                onLoaded.Invoke(assetAsT);
            });
        }
    }
}