// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Async.Threads;
using Depra.Assets.Runtime.Async.Tokens;
using Depra.Assets.Runtime.Files.Interfaces;
using Depra.Assets.Runtime.Files.Structs;
using Depra.Assets.Runtime.Utils;
using Depra.Coroutines.Domain.Entities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Files.Group
{
    public sealed class AssetGroup :
        ILoadableAsset<IEnumerable<Object>>,
        IEnumerable<ILoadableAsset<Object>>,
        IDisposable
    {
        private readonly List<Object> _loadedAssets;
        private readonly ICoroutineHost _coroutineHost;
        private readonly List<ILoadableAsset<Object>> _childAssets;

        public AssetGroup(string name = "", string path = "", List<ILoadableAsset<Object>> children = null,
            ICoroutineHost coroutineHost = null)
        {
            Name = name;
            Path = path;
            _childAssets = children ?? new List<ILoadableAsset<Object>>();
            _loadedAssets = new List<Object>(_childAssets.Count);
            _coroutineHost = coroutineHost ?? AssetCoroutineHook.Instance;
        }

        public string Name { get; }
        public string Path { get; }

        public bool IsLoaded => _childAssets.All(asset => asset.IsLoaded);
        public FileSize Size => new(_childAssets.Sum(x => x.Size.SizeInBytes));

        public void AddAsset(ILoadableAsset<Object> asset)
        {
            if (_childAssets.Contains(asset))
            {
                throw new InvalidOperationException($"Asset {asset.Name} already added to group!");
            }

            _childAssets.Add(asset);
        }

        public IEnumerable<Object> Load()
        {
            foreach (var asset in _childAssets)
            {
                if (asset.IsLoaded)
                {
                    yield return asset.Load();
                }

                var loadedAsset = asset.Load();
                OnLoaded(loadedAsset, onFailed: exception => throw exception);

                yield return loadedAsset;
            }
        }

        public IAsyncToken LoadAsync(Action<IEnumerable<Object>> onLoaded, Action<DownloadProgress> onProgress = null,
            Action<Exception> onFailed = null)
        {
            if (IsLoaded)
            {
                return AlreadyLoadedAsset<IEnumerable<Object>>.Create(_loadedAssets, onLoaded, onProgress);
            }

            var loadingThread = new Thread(_coroutineHost, _childAssets, OnLoadedInternal);
            loadingThread.Start(OnChildLoaded, onProgress, onFailed);
            return new AsyncActionToken(loadingThread.Cancel);

            void OnLoadedInternal() => onLoaded?.Invoke(_loadedAssets);

            void OnChildLoaded(Object loadedAsset)
            {
                OnLoaded(loadedAsset, onFailed);
                OnProgressChanged();
            }

            void OnProgressChanged()
            {
                var progressValue = (float)_loadedAssets.Count / _childAssets.Count;
                var progress = new DownloadProgress(progressValue);
                onProgress?.Invoke(progress);
            }
        }

        public void Unload()
        {
            _loadedAssets.Clear();
            foreach (var asset in _childAssets)
            {
                asset.Unload();
            }
        }

        private void OnLoaded(Object loadedAsset, Action<Exception> onFailed)
        {
            EnsureAsset(loadedAsset, onFailed);
            if (_loadedAssets.Contains(loadedAsset))
            {
                onFailed?.Invoke(new InvalidOperationException("Asset already loaded!"));
            }
            else
            {
                _loadedAssets.Add(loadedAsset);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureAsset(Object loadedAsset, Action<Exception> onFailed)
        {
            if (loadedAsset == null)
            {
                onFailed?.Invoke(new AssetGroupLoadingException(Name));
            }
        }

        public IEnumerator<ILoadableAsset<Object>> GetEnumerator() => _childAssets.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void IDisposable.Dispose() => Unload();

        private sealed class Thread : IAssetThread<Object>
        {
            private readonly Action _onLoaded;
            private readonly ICoroutineHost _coroutineHost;
            private readonly IEnumerable<ILoadableAsset<Object>> _childAssets;

            private ICoroutine _coroutine;

            public Thread(ICoroutineHost coroutineHost, IEnumerable<ILoadableAsset<Object>> assets, Action onLoaded)
            {
                _childAssets = assets ?? throw new ArgumentNullException(nameof(assets));
                _onLoaded = onLoaded ?? throw new ArgumentNullException(nameof(onLoaded));
                _coroutineHost = coroutineHost ?? throw new ArgumentNullException(nameof(coroutineHost));
            }

            public void Start(
                Action<Object> onChildLoaded,
                Action<DownloadProgress> onProgress,
                Action<Exception> onFailed) =>
                _coroutine = _coroutineHost.StartCoroutine(LoadingProcess(onChildLoaded, onFailed));

            public void Cancel() => _coroutine?.Stop();

            private IEnumerator LoadingProcess(Action<Object> onChildLoaded, Action<Exception> onFailed)
            {
                foreach (var childAsset in _childAssets)
                {
                    if (childAsset.IsLoaded)
                    {
                        continue;
                    }

                    childAsset.LoadAsync(onChildLoaded, onFailed: InvokeCancel);
                    yield return new WaitUntil(() => childAsset.IsLoaded);
                }

                _onLoaded.Invoke();

                void InvokeCancel(Exception exception)
                {
                    onFailed?.Invoke(exception);
                    Cancel();
                }
            }
        }
    }
}