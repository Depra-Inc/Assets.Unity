using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Async.Tokens;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Assets.Runtime.Files.Resource.Exceptions;
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
        public FileSize Size { get; private set; } = FileSize.Unknown;

        public TAsset Load()
        {
            if (IsLoaded)
            {
                return _loadedAsset;
            }

            var asset = Resources.Load<TAsset>(Path);
            return OnLoaded(asset, onFailed: exception => throw exception);
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

        public IAsyncToken LoadAsync(Action<TAsset> onLoaded, Action<float> onProgress = null,
            Action<Exception> onFailed = null)
        {
            if (IsLoaded)
            {
                onProgress?.Invoke(1f);
                onLoaded.Invoke(_loadedAsset);
                return AsyncActionToken.Empty;
            }

            var loadingCoroutine = new AssetFileLoadingCoroutine(_coroutineHost);
            var asyncToken = new AsyncActionToken(loadingCoroutine.Cancel);
            onLoaded += _ => asyncToken.Complete();
            loadingCoroutine.Start(LoadingProcess(
                onLoaded: asset => OnLoaded(asset, onFailed, onLoaded),
                onProgress: onProgress));
            
            return asyncToken;
        }

        private TAsset OnLoaded(TAsset loadedAsset, Action<Exception> onFailed, Action<TAsset> onLoaded = null)
        {
            Ensure(loadedAsset, onFailed);
            _loadedAsset = loadedAsset;
            onLoaded?.Invoke(_loadedAsset);
            RefreshSize(_loadedAsset);

            return _loadedAsset;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerator LoadingProcess(Action<TAsset> onLoaded, Action<float> onProgress = null)
        {
            var request = Resources.LoadAsync<TAsset>(Path);
            while (request.isDone == false)
            {
                onProgress?.Invoke(request.progress);
                yield return null;
            }

            onProgress?.Invoke(1f);
            onLoaded.Invoke((TAsset)request.asset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RefreshSize(TAsset asset) =>
            Size = new FileSize(Profiler.GetRuntimeMemorySizeLong(asset));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Ensure(TAsset asset, Action<Exception> onFailed)
        {
            if (asset == null)
            {
                onFailed?.Invoke(new ResourceNotLoadedException(Path));
            }
        }

        void IDisposable.Dispose() => Unload();
    }
}