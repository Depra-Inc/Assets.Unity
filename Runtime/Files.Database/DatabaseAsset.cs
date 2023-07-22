// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Delegates;
using Depra.Assets.Idents;
using Depra.Assets.Unity.Runtime.Common;
using Depra.Assets.Unity.Runtime.Exceptions;
using Depra.Assets.Unity.Runtime.Extensions;
using Depra.Assets.Unity.Runtime.Files.Adapter;
using Depra.Assets.ValueObjects;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Unity.Runtime.Files.Database
{
    public sealed class DatabaseAsset<TAsset> : UnityAssetFile<TAsset>, IDisposable where TAsset : ScriptableObject
    {
        public static implicit operator TAsset(DatabaseAsset<TAsset> from) => from.Load();
        private static Type AssetType => typeof(TAsset);

        private readonly DatabaseAssetIdent _ident;
        private TAsset _loadedAsset;

        public DatabaseAsset(DatabaseAssetIdent ident) =>
            _ident = ident;

        public override IAssetIdent Ident => _ident;
        public override bool IsLoaded => _loadedAsset != null;
        public override FileSize Size { get; protected set; } = FileSize.Unknown;

        public override TAsset Load()
        {
            if (IsLoaded)
            {
                return _loadedAsset;
            }

            TAsset loadedAsset = null;
#if UNITY_EDITOR
            if (File.Exists(_ident.AbsolutePath))
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
            Size = UnityFileSize.FromProfiler(_loadedAsset);

            return _loadedAsset;
        }

        public override void Unload()
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

        [Obsolete("Not yet supported in Unity. Use DatabaseAsset<TAsset>.Load() instead")]
        public override UniTask<TAsset> LoadAsync(DownloadProgressDelegate onProgress = null,
            CancellationToken cancellationToken = default)
        {
            if (IsLoaded)
            {
                onProgress?.Invoke(DownloadProgress.Full);
                return UniTask.FromResult(_loadedAsset);
            }

            throw new AssetCanNotBeLoaded("Asynchronous loading is not supported by Unity");
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
            _ident.Directory.CreateIfNotExists();

            asset.name = _ident.Name;
            AssetDatabase.CreateAsset(asset, _ident.RelativePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return asset;
        }
#endif

        void IDisposable.Dispose() => Unload();
    }
}