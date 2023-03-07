using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Exceptions;
using Depra.Assets.Runtime.Interfaces.Files;
using Depra.Assets.Runtime.Interfaces.Requests;
using Depra.Assets.Runtime.Utils;
using Depra.Coroutines.Runtime;
using UnityEngine;
using static Depra.Assets.Runtime.Common.Static;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Bundle.Files
{
    public sealed class AssetBundleAssetFile<TAsset> : ILoadableAsset<TAsset>, IDisposable where TAsset : Object
    {
        private readonly AssetBundleFile _assetBundle;
        private readonly ICoroutineHost _coroutineHost;

        private TAsset _loadedAsset;

        public AssetBundleAssetFile(AssetBundleFile assetBundle, string name, ICoroutineHost coroutineHost = null)
        {
            Name = name;
            _assetBundle = assetBundle ?? throw new ArgumentNullException(nameof(assetBundle));
            Path = CombineIntoPath(_assetBundle.Path, Name);
            _coroutineHost = coroutineHost ?? AssetCoroutineHook.Instance;
        }

        public string Name { get; }
        public string Path { get; }
        public bool IsLoaded => _loadedAsset != null;

        public TAsset Load()
        {
            if (IsLoaded)
            {
                return _loadedAsset;
            }

            var assetAsT = _assetBundle.Load().LoadAsset<TAsset>(Name);
            EnsureAsset(assetAsT);

            return assetAsT;
        }

        public void Unload()
        {
            if (IsLoaded)
            {
                _loadedAsset = null;
            }
        }

        public void LoadAsync(IAssetLoadingCallbacks<TAsset> callbacks)
        {
            if (IsLoaded)
            {
                callbacks.InvokeProgressEvent(1f);
                callbacks.InvokeLoadedEvent(_loadedAsset);
            }

            var assetBundleLoadingCallbacks = new AssetLoadingCallbacks<AssetBundle>(
                onLoaded: SendRequest,
                onFailed: exception => throw exception);

            _assetBundle.LoadAsync(assetBundleLoadingCallbacks);

            void SendRequest(AssetBundle bundle)
            {
                var request = new Request(by: Path, from: bundle, _coroutineHost);
                request.Send(new ReturnedAssetLoadingCallbacks<TAsset>(
                    asset => _loadedAsset = asset,
                    new GuardedAssetLoadingCallbacks<TAsset>(callbacks, EnsureAsset)));
            }
        }

        public void Dispose() => Unload();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureAsset(TAsset asset)
        {
            if (asset == null)
            {
                throw new AssetLoadingException(typeof(TAsset), Path);
            }
        }

        public static implicit operator TAsset(AssetBundleAssetFile<TAsset> assetFile) => assetFile.Load();

        private sealed class Request : ITypedAssetRequest<TAsset>
        {
            private readonly string _path;
            private readonly AssetBundle _bundle;
            private readonly ICoroutineHost _coroutineHost;

            private ICoroutine _loadCoroutine;

            public Request(string by, AssetBundle from, ICoroutineHost coroutineHost)
            {
                _path = by;
                _bundle = from;
                _coroutineHost = coroutineHost;
            }

            public bool Done { get; private set; }
            public bool Running => _loadCoroutine != null;

            public void Send(IAssetLoadingCallbacks<TAsset> callbacks)
            {
                _loadCoroutine = _coroutineHost.StartCoroutine(RequestCoroutine(callbacks));
            }

            public void Cancel()
            {
                if (Running)
                {
                    Clean();
                }
            }

            public void Destroy()
            {
                if (Done || Running)
                {
                    Clean();
                }
            }

            private IEnumerator RequestCoroutine(IAssetLoadingCallbacks<TAsset> callbacks)
            {
                var request = _bundle.LoadAssetAsync<TAsset>(_path);
                while (request.isDone == false)
                {
                    callbacks.InvokeProgressEvent(request.progress);
                    yield return null;
                }

                callbacks.InvokeLoadedEvent((TAsset)request.asset);
                Done = true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void Clean()
            {
                Done = false;
                _coroutineHost.StopCoroutine(_loadCoroutine);
                _loadCoroutine = null;
            }

            void IDisposable.Dispose() => Clean();
        }
    }
}