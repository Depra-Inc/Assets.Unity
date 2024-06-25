// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Depra.Asset.Delegates;
using Depra.Asset.Exceptions;
using Depra.Asset.Files.Bundles.Exceptions;
using Depra.Asset.Files.Bundles.Extensions;
using Depra.Asset.ValueObjects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Asset.Files.Bundles
{
	public sealed class AssetBundleAssetFile<TAsset> : IAssetFile<TAsset>, IDisposable where TAsset : Object
	{
		private readonly AssetBundle _assetBundle;
		private TAsset _loadedAsset;

		public AssetBundleAssetFile(AssetName name, AssetBundle assetBundle)
		{
			Guard.AgainstNull(name, nameof(name));
			Guard.AgainstNull(assetBundle, nameof(assetBundle));

			_assetBundle = assetBundle;
			Metadata = new AssetMetadata(name, FileSize.Unknown);
		}

		public AssetMetadata Metadata { get; }
		public bool IsLoaded => _loadedAsset != null;
		private string Name => Metadata.Uri.Relative;

		public TAsset Load()
		{
			if (IsLoaded)
			{
				return _loadedAsset;
			}

			var loadedAsset = _assetBundle.LoadAsset<TAsset>(Name);
			Guard.AgainstNull(loadedAsset, () => new AssetBundleFileNotLoaded(Name, _assetBundle.name));

			_loadedAsset = loadedAsset;
			Metadata.Size = UnityFileSize.FromProfiler(_loadedAsset);

			return _loadedAsset;
		}

		public void Unload()
		{
			if (IsLoaded)
			{
				_loadedAsset = null;
			}
		}

		public async Task<TAsset> LoadAsync(DownloadProgressDelegate onProgress = null,
			CancellationToken cancellationToken = default)
		{
			if (IsLoaded)
			{
				onProgress?.Invoke(DownloadProgress.Full);
				return _loadedAsset;
			}

			var loadedAsset = await _assetBundle
				.LoadAssetAsync<TAsset>(Name)
				.ToTask(OnProgress, cancellationToken);

			Guard.AgainstNull(loadedAsset, () => new AssetBundleFileNotLoaded(Name, _assetBundle.name));

			_loadedAsset = (TAsset) loadedAsset;
			onProgress?.Invoke(DownloadProgress.Full);
			Metadata.Size = UnityFileSize.FromProfiler(_loadedAsset);

			return _loadedAsset;

			void OnProgress(float progress) => onProgress?.Invoke(new DownloadProgress(progress));
		}

		public IEnumerable<IAssetUri> Dependencies() => AssetBundleDependenciesExtractor.Extract(_assetBundle);

		void IDisposable.Dispose() => Unload();
	}
}