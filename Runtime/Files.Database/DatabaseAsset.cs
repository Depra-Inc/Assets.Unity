// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Depra.Assets.Delegates;
using Depra.Assets.Exceptions;
using Depra.Assets.Extensions;
using Depra.Assets.ValueObjects;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Files.Database
{
	public sealed class DatabaseAsset<TAsset> : IAssetFile<TAsset>, IDisposable where TAsset : ScriptableObject
	{
		private readonly Type _assetType;
		private readonly DatabaseAssetUri _uri;

		private TAsset _loadedAsset;

		public DatabaseAsset(DatabaseAssetUri uri)
		{
			_assetType = typeof(TAsset);
			Metadata = new AssetMetadata(_uri = uri, FileSize.Unknown);
		}

		public AssetMetadata Metadata { get; }
		public bool IsLoaded => _loadedAsset != null;

		public TAsset Load()
		{
			if (IsLoaded)
			{
				return _loadedAsset;
			}

			TAsset loadedAsset = null;
#if UNITY_EDITOR
			if (_uri.Exists())
			{
				loadedAsset = AssetDatabase.LoadAssetAtPath<TAsset>(_uri.Relative);
			}
#endif
			if (loadedAsset == null)
			{
				loadedAsset = CreateAsset();
			}

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

#if UNITY_EDITOR
			AssetDatabase.DeleteAsset(_uri.Relative);
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
			_uri.Directory.CreateIfNotExists();

			asset.name = _uri.Name;
			AssetDatabase.CreateAsset(asset, _uri.Relative);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			return asset;
		}
#endif
		IEnumerable<IAssetUri> IAssetFile.Dependencies()
		{
#if UNITY_EDITOR
			var paths = AssetDatabase.GetDependencies(_uri.Relative);
			foreach (var path in paths)
			{
				yield return new DatabaseAssetUri(path);
			}
#else
			return Array.Empty<IAssetUri>();
#endif
		}

		void IDisposable.Dispose() => Unload();
	}
}