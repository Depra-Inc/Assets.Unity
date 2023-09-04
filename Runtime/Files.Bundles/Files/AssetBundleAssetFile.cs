// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Threading;
using System.Threading.Tasks;
using Depra.Assets.Delegates;
using Depra.Assets.Files;
using Depra.Assets.Idents;
using Depra.Assets.Runtime.Exceptions;
using Depra.Assets.Runtime.Files.Bundles.Exceptions;
using Depra.Assets.Runtime.Files.Bundles.Extensions;
using Depra.Assets.ValueObjects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Files.Bundles.Files
{
	public sealed class AssetBundleAssetFile<TAsset> : ILoadableAsset<TAsset>, IDisposable where TAsset : Object
	{
		public static implicit operator TAsset(AssetBundleAssetFile<TAsset> self) => self.Load();

		private readonly AssetName _ident;
		private readonly AssetBundle _assetBundle;

		private TAsset _loadedAsset;

		public AssetBundleAssetFile(AssetName name, AssetBundle assetBundle)
		{
			_ident = name ?? throw new ArgumentNullException(nameof(name));
			_assetBundle = assetBundle ? assetBundle : throw new ArgumentNullException(nameof(assetBundle));
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

			var loadedAsset = _assetBundle.LoadAsset<TAsset>(_ident.Name);
			Guard.AgainstNull(loadedAsset,
				() => new AssetBundleFileNotLoaded(_ident.Name, _assetBundle.name));

			_loadedAsset = loadedAsset;
			Size = UnityFileSize.FromProfiler(_loadedAsset);

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
				.LoadAssetAsync<TAsset>(_ident.Name)
				.ToTask(progress => onProgress?.Invoke(new DownloadProgress(progress)), cancellationToken);

			Guard.AgainstNull(loadedAsset, () => new AssetBundleFileNotLoaded(_ident.Name, _assetBundle.name));

			_loadedAsset = (TAsset) loadedAsset;
			onProgress?.Invoke(DownloadProgress.Full);
			Size = UnityFileSize.FromProfiler(_loadedAsset);

			return _loadedAsset;
		}

		void IDisposable.Dispose() => Unload();
	}
}