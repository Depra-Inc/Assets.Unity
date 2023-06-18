// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Runtime.Exceptions;
using Depra.Assets.Runtime.Extensions;
using Depra.Assets.Runtime.Files.Delegates;
using Depra.Assets.Runtime.Files.Idents;
using Depra.Assets.Runtime.Files.Interfaces;
using Depra.Assets.Runtime.Files.ValueObjects;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Files.Database
{
    public sealed class DatabaseAsset<TAsset> : ILoadableAsset<TAsset>, IDisposable where TAsset : ScriptableObject
    {
        private static Type AssetType => typeof(TAsset);

        private readonly DatabaseAssetIdent _ident;

        private TAsset _loadedAsset;

        public DatabaseAsset(DatabaseAssetIdent ident) =>
            _ident = ident;

        public IAssetIdent Ident => _ident;
        public string Name => _ident.Name;
        public string AbsolutePath => _ident.AbsolutePath;

        public bool IsLoaded => _loadedAsset != null;
        public FileSize Size { get; private set; } = FileSize.Unknown;

        public TAsset Load()
        {
            if (IsLoaded)
            {
                return _loadedAsset;
            }

            TAsset loadedAsset = null;
#if UNITY_EDITOR
            if (File.Exists(AbsolutePath))
            {
                loadedAsset = AssetDatabase.LoadAssetAtPath<TAsset>(_ident.RelativePath);
            }
#endif
            if (loadedAsset == null)
            {
                loadedAsset = CreateAsset();
            }

            Guard.AgainstNull(loadedAsset, () => new AssetCreationException(AssetType, AssetType.Name));

            _loadedAsset = loadedAsset;
            Size = FileSize.FromProfiler(_loadedAsset);

            return _loadedAsset;
        }

        public void Unload()
        {
            if (IsLoaded == false)
            {
                return;
            }

#if UNITY_EDITOR
            AssetDatabase.DeleteAsset(_ident.RelativePath);
#endif
            _loadedAsset = null;
        }

        public async UniTask<TAsset> LoadAsync(DownloadProgressDelegate onProgress = null,
            CancellationToken cancellationToken = default)
        {
            if (IsLoaded)
            {
                onProgress?.Invoke(DownloadProgress.Full);

                return _loadedAsset;
            }

            await UniTask.SwitchToMainThread(cancellationToken: cancellationToken);
            var loadedAsset = await UniTask.RunOnThreadPool(Load, configureAwait: false, cancellationToken);

            onProgress?.Invoke(DownloadProgress.Full);

            Guard.AgainstNull(loadedAsset, () => new AssetCreationException(AssetType, AssetType.Name));

            _loadedAsset = loadedAsset;
            Size = FileSize.FromProfiler(_loadedAsset);

            return _loadedAsset;
        }

        private TAsset CreateAsset()
        {
            var asset = ScriptableObject.CreateInstance<TAsset>();
#if UNITY_EDITOR
            asset = (TAsset) ActivateAsset(asset);
#endif

            return asset;
        }

#if UNITY_EDITOR
        private Object ActivateAsset(Object asset)
        {
            _ident.AbsoluteDirectory.CreateIfNotExists();

            asset.name = Name;
            AssetDatabase.CreateAsset(asset, _ident.RelativePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return asset;
        }
#endif

        void IDisposable.Dispose() => Unload();
    }
}