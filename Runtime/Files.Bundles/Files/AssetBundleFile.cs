// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Threading;
using System.Threading.Tasks;
using Depra.Assets.Delegates;
using Depra.Assets.Exceptions;
using Depra.Assets.Files.Bundles.Exceptions;
using Depra.Assets.Files.Bundles.Sources;
using Depra.Assets.ValueObjects;
using UnityEngine;

namespace Depra.Assets.Files.Bundles
{
	public sealed class AssetBundleFile : IAssetFile<AssetBundle>, IDisposable
	{
		public static implicit operator AssetBundle(AssetBundleFile self) => self.Load();

		private readonly AssetBundleUri _uri;
		private readonly IAssetBundleSource _source;

		private AssetBundle _loadedAssetBundle;

		public AssetBundleFile(AssetBundleUri uri, IAssetBundleSource source)
		{
			Guard.AgainstNull(uri, () => new ArgumentNullException(nameof(uri)));
			Guard.AgainstNull(source, () => new ArgumentNullException(nameof(source)));

			_source = source;
			Metadata = new AssetMetadata(_uri = uri, FileSize.Unknown);
		}

		public AssetMetadata Metadata { get; }
		public bool IsLoaded => _loadedAssetBundle != null;

		public AssetBundle Load()
		{
			if (IsLoaded)
			{
				return _loadedAssetBundle;
			}

			var loadedAssetBundle = _source.Load(by: _uri.AbsolutePathWithoutExtension);
			Guard.AgainstNull(loadedAssetBundle, () => new AssetBundleNotLoaded(_uri.AbsolutePath));

			_loadedAssetBundle = loadedAssetBundle;
			Metadata.Size = _source.Size(of: _loadedAssetBundle);

			return _loadedAssetBundle;
		}

		public async Task<AssetBundle> LoadAsync(DownloadProgressDelegate onProgress = null,
			CancellationToken cancellationToken = default)
		{
			if (IsLoaded)
			{
				onProgress?.Invoke(DownloadProgress.Full);
				return _loadedAssetBundle;
			}

			var loadedAssetBundle = await _source.LoadAsync(by: _uri.AbsolutePathWithoutExtension,
				withProgress: progress => onProgress?.Invoke(new DownloadProgress(progress)),
				cancellationToken);

			Guard.AgainstNull(loadedAssetBundle, () => new AssetBundleNotLoaded(_uri.AbsolutePath));

			_loadedAssetBundle = loadedAssetBundle;
			onProgress?.Invoke(DownloadProgress.Full);
			Metadata.Size = _source.Size(of: _loadedAssetBundle);

			return _loadedAssetBundle;
		}

		public void Unload()
		{
			if (IsLoaded == false)
			{
				return;
			}

			_loadedAssetBundle.Unload(true);
			_loadedAssetBundle = null;
		}

		public void UnloadAsync()
		{
			if (IsLoaded == false)
			{
				return;
			}

			_loadedAssetBundle.UnloadAsync(true);
			_loadedAssetBundle = null;
		}

		void IDisposable.Dispose() => Unload();
	}
}