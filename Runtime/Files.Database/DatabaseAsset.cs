// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using System.Threading.Tasks;
using Depra.Assets.Delegates;
using Depra.Assets.Files;
using Depra.Assets.Idents;
using Depra.Assets.Runtime.Exceptions;
using Depra.Assets.Runtime.Extensions;
using Depra.Assets.ValueObjects;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Files.Database
{
	public sealed class DatabaseAsset<TAsset> : ILoadableAsset<TAsset>, IDisposable where TAsset : ScriptableObject
	{
		public static implicit operator TAsset(DatabaseAsset<TAsset> from) => from.Load();

		private readonly Type _assetType;
		private readonly DatabaseAssetIdent _ident;

		private TAsset _loadedAsset;

		public DatabaseAsset(DatabaseAssetIdent ident)
		{
			_ident = ident;
			_assetType = typeof(TAsset);
		}

		public IAssetIdent Ident => _ident;
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
			if (_ident.Exists())
			{
				loadedAsset = AssetDatabase.LoadAssetAtPath<TAsset>(_ident.RelativePath);
			}
#endif
			if (loadedAsset == null)
			{
				loadedAsset = CreateAsset();
			}

			Guard.AgainstNull(loadedAsset, () => new AssetCatNotBeCreated(_assetType, _assetType.Name));

			_loadedAsset = loadedAsset;
			Size = UnityFileSize.FromProfiler(_loadedAsset);

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

		[Obsolete("Not yet supported in Unity. Use DatabaseAsset<TAsset>.Load() instead")]
		public Task<TAsset> LoadAsync(DownloadProgressDelegate onProgress = null,
			CancellationToken cancellationToken = default)
		{
			if (IsLoaded)
			{
				onProgress?.Invoke(DownloadProgress.Full);
				return Task.FromResult(_loadedAsset);
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