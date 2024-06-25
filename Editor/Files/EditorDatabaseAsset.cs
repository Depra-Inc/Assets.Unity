// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Depra.Asset.Delegates;
using Depra.Asset.Exceptions;
using Depra.Asset.Extensions;
using Depra.Asset.Files;
using Depra.Asset.Files.Database;
using Depra.Asset.ValueObjects;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Asset.Editor.Files
{
	public sealed class EditorDatabaseAsset<TAsset> : IAssetFile<TAsset>, IDisposable where TAsset : ScriptableObject
	{
		private readonly Type _assetType;
		private readonly DatabaseAssetUri _uri;
		private readonly RuntimeDatabaseAsset<TAsset> _runtimeAsset;

		private TAsset _loadedAsset;

		public EditorDatabaseAsset(DatabaseAssetUri uri)
		{
			_uri = uri;
			_assetType = typeof(TAsset);
			Metadata = new AssetMetadata(_uri, FileSize.Unknown);
			_runtimeAsset = new RuntimeDatabaseAsset<TAsset>(_uri);
		}

		public AssetMetadata Metadata { get; }
		public bool IsLoaded => _loadedAsset != null;

		public TAsset Load()
		{
			if (IsLoaded)
			{
				return _loadedAsset;
			}

			var loadedAsset = _uri.Exists()
				? AssetDatabase.LoadAssetAtPath<TAsset>(_uri.Relative)
				: (TAsset) ActivateAsset(_runtimeAsset.Load());

			Guard.AgainstNull(loadedAsset, () => new AssetCatNotBeCreated(_assetType, _assetType.Name));

			_loadedAsset = loadedAsset;
			Metadata.Size = UnityFileSize.FromProfiler(_loadedAsset);

			return _loadedAsset;
		}

		public void Unload()
		{
			if (IsLoaded == false)
			{
				return;
			}

			_loadedAsset = null;
			_runtimeAsset.Unload();
			AssetDatabase.DeleteAsset(_uri.Relative);
		}

		private Object ActivateAsset(Object asset)
		{
			_uri.Directory.Require();

			asset.name = _uri.Name;
			AssetDatabase.CreateAsset(asset, _uri.Relative);
			AssetDatabase.SaveAssets();

			return asset;
		}

		public IEnumerable<IAssetUri> Dependencies() => AssetDatabase
			.GetDependencies(_uri.Relative, recursive: false)
			.Select(path => new DatabaseAssetUri(path)).Cast<IAssetUri>();

		void IDisposable.Dispose() => Unload();

		[Obsolete("Not yet supported in Unity. Use DatabaseAsset<TAsset>.Load() instead")]
		Task<TAsset> IAssetFile<TAsset>.LoadAsync(DownloadProgressDelegate onProgress,
			CancellationToken cancellationToken)
		{
			onProgress?.Invoke(DownloadProgress.Full);
			return Task.FromResult(IsLoaded ? _loadedAsset : Load());
		}
	}
}