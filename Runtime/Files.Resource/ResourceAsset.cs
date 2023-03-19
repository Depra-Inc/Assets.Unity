using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Bundle.Files;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Files.Resource.Exceptions;
using Depra.Assets.Runtime.Internal.Patterns;
using Depra.Assets.Runtime.Utils;
using Depra.Coroutines.Domain.Entities;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Files.Resource
{
    public sealed class ResourceAsset<TAsset> : ILoadableAsset<TAsset>, IDisposable where TAsset : Object
    {
        private readonly AssetIdent _ident;
        private readonly ICoroutineHost _coroutineHost;

        private TAsset _loadedAsset;

        public ResourceAsset(AssetIdent ident, ICoroutineHost coroutineHost = null)
        {
            _ident = ident;
            _coroutineHost = coroutineHost ?? AssetCoroutineHook.Instance;
        }

        public string Name => _ident.Name;
        public string Path => _ident.Path;

        public bool IsLoaded => _loadedAsset != null;

        public FileSize Size => IsLoaded
            ? new FileSize(Profiler.GetRuntimeMemorySizeLong(_loadedAsset))
            : throw new ResourceNotLoadedException(Path);

        public TAsset Load()
        {
            if (IsLoaded)
            {
                return _loadedAsset;
            }

            var asset = Resources.Load<TAsset>(Path);
            EnsureAsset(asset, exception => throw exception);
            _loadedAsset = asset;

            return _loadedAsset;
        }

        public IDisposable LoadAsync(IAssetLoadingCallbacks<TAsset> callbacks)
        {
            if (IsLoaded)
            {
                callbacks.InvokeProgressEvent(1f);
                callbacks.InvokeLoadedEvent(_loadedAsset);
                return new EmptyDisposable();
            }

            var loadingCoroutine = new AssetFileLoadingCoroutine(_coroutineHost);
            var loadingOperation = LoadingProcess(callbacks
                .AddGuard(asset => EnsureAsset(asset, callbacks.InvokeFailedEvent))
                .ReturnTo(asset => _loadedAsset = asset));

            loadingCoroutine.Start(loadingOperation);

            return loadingCoroutine;
        }

        public void Unload()
        {
            if (IsLoaded == false)
            {
                return;
            }

            Resources.UnloadAsset(_loadedAsset);
            _loadedAsset = null;
        }

        private IEnumerator LoadingProcess(IAssetLoadingCallbacks<TAsset> callbacks)
        {
            var request = Resources.LoadAsync<TAsset>(Path);
            while (request.isDone == false)
            {
                callbacks.InvokeProgressEvent(request.progress);
                yield return null;
            }

            callbacks.InvokeLoadedEvent((TAsset)request.asset);
            callbacks.InvokeProgressEvent(1f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureAsset(TAsset asset, Action<Exception> onFailed)
        {
            if (asset == null)
            {
                onFailed?.Invoke(new ResourceLoadingException(typeof(TAsset).Name, Path));
            }
        }

        void IDisposable.Dispose() => Unload();
    }
}