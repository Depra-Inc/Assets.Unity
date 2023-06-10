// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Runtime.Exceptions;
using Depra.Assets.Runtime.Extensions;
using Depra.Assets.Runtime.Files.Idents;
using Depra.Assets.Runtime.Files.Interfaces;
using Depra.Assets.Runtime.Files.Resource;
using Depra.Assets.Runtime.Files.Structs;
using UnityEditor;
using UnityEngine;
using static Depra.Assets.Runtime.Common.Constants;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Files.Database
{
    public sealed class DatabaseAsset<TAsset> : ILoadableAsset<TAsset>, IDisposable where TAsset : ScriptableObject
    {
        private readonly Type _assetType;
        private readonly string _absoluteFilePath;
        private readonly DirectoryInfo _absoluteDirectory;

        private TAsset _loadedAsset;

        public DatabaseAsset(FileSystemAssetIdent ident)
        {
            Name = ident.Name;
            _assetType = typeof(TAsset);
            _absoluteDirectory = new DirectoryInfo(System.IO.Path.Combine(Application.dataPath, ident.Directory));
            var nameWithExtension = Name + ident.Extension;
            _absoluteFilePath = System.IO.Path.Combine(_absoluteDirectory.FullName, nameWithExtension);
            var projectPath = System.IO.Path.Combine(ASSETS_FOLDER_NAME, ident.Directory, nameWithExtension);
            Path = projectPath;
        }

        public string Name { get; }
        public string Path { get; }

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
            if (File.Exists(_absoluteFilePath))
            {
                loadedAsset = AssetDatabase.LoadAssetAtPath<TAsset>(Path);
            }
#endif
            if (loadedAsset == null)
            {
                loadedAsset = CreateAsset();
            }

            Guard.AgainstNull(loadedAsset, () => new AssetCreationException(_assetType, _assetType.Name));

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
            AssetDatabase.DeleteAsset(Path);
#endif
            _loadedAsset = null;
        }

        public async UniTask<TAsset> LoadAsync(CancellationToken cancellationToken,
            DownloadProgressDelegate onProgress = null)
        {
            if (IsLoaded)
            {
                onProgress?.Invoke(DownloadProgress.Full);

                return _loadedAsset;
            }

            await UniTask.SwitchToMainThread(cancellationToken: cancellationToken);
            var loadedAsset = await UniTask.RunOnThreadPool(Load, configureAwait: false, cancellationToken);

            onProgress?.Invoke(DownloadProgress.Full);

            Guard.AgainstNull(loadedAsset, () => new AssetCreationException(_assetType, _assetType.Name));

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
            _absoluteDirectory.CreateIfNotExists();

            asset.name = Name;
            AssetDatabase.CreateAsset(asset, Path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return asset;
        }
#endif

        void IDisposable.Dispose() => Unload();
    }
}