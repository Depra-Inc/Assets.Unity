// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Depra.Assets.Delegates;
using Depra.Assets.Exceptions;
using Depra.Assets.Files;
using Depra.Assets.ValueObjects;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Depra.Assets.Editor.Files
{
	public sealed class PreloadedAsset<TAsset> : IAssetFile<TAsset>, IDisposable where TAsset : Object
	{
		private readonly Type _assetType;
		private readonly IAssetFile<TAsset> _asset;

		private TAsset _loadedAsset;

		public PreloadedAsset(IAssetFile<TAsset> asset)
		{
			Guard.AgainstNull(asset, nameof(asset));

			_asset = asset;
			_assetType = typeof(TAsset);
		}

		public bool IsLoaded => _loadedAsset != null;
		public AssetMetadata Metadata => _asset.Metadata;

		public TAsset Load()
		{
			if (IsLoaded)
			{
				return _loadedAsset;
			}

			if (TryGetPreloadedAsset(out var loadedAsset) == false &&
			    TryLoadAssetFromDatabase(out loadedAsset) == false)
			{
				loadedAsset = _asset.Load();
			}
			else
			{
				Metadata.Size = UnityFileSize.FromProfiler(loadedAsset);
			}

			return _loadedAsset = loadedAsset;
		}

		public void Unload()
		{
			if (IsLoaded == false)
			{
				return;
			}

			_asset.Unload();
			_loadedAsset = null;
		}

		public async Task<TAsset> LoadAsync(DownloadProgressDelegate onProgress = null,
			CancellationToken cancellationToken = default)
		{
			if (IsLoaded)
			{
				onProgress?.Invoke(DownloadProgress.Full);
				return _loadedAsset;
			}

			if (TryGetPreloadedAsset(out var loadedAsset) == false &&
			    TryLoadAssetFromDatabase(out loadedAsset) == false)
			{
				loadedAsset = await _asset.LoadAsync(onProgress, cancellationToken);
			}
			else
			{
				Metadata.Size = UnityFileSize.FromProfiler(loadedAsset);
			}

			return _loadedAsset = loadedAsset;
		}

		public IEnumerable<IAssetUri> Dependencies() => _asset.Dependencies();

		private bool TryGetPreloadedAsset(out TAsset preloadedAsset)
		{
			var assetByType = PlayerSettings
				.GetPreloadedAssets()
				.FirstOrDefault(asset => asset.GetType() == _assetType);

			if (assetByType == null)
			{
				preloadedAsset = null;
				return false;
			}

			preloadedAsset = (TAsset) assetByType;
			return preloadedAsset != null;
		}

		private bool TryLoadAssetFromDatabase(out TAsset asset)
		{
			var assetGuid = AssetDatabase
				.FindAssets(filter: $"t:{_assetType.Name}")
				.FirstOrDefault();

			var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
			asset = AssetDatabase.LoadAssetAtPath<TAsset>(assetPath);

			return asset != null;
		}

		void IDisposable.Dispose() => Unload();
	}
}