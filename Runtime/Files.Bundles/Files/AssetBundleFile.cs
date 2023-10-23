// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Threading;
using System.Threading.Tasks;
using Depra.Assets.Delegates;
using Depra.Assets.Files;
using Depra.Assets.Idents;
using Depra.Assets.Exceptions;
using Depra.Assets.Files.Bundles.Exceptions;
using Depra.Assets.Files.Bundles.Idents;
using Depra.Assets.Files.Bundles.Sources;
using Depra.Assets.ValueObjects;
using UnityEngine;

namespace Depra.Assets.Files.Bundles.Files
{
	public sealed class AssetBundleFile : ILoadableAsset<AssetBundle>, IDisposable
	{
		public static implicit operator AssetBundle(AssetBundleFile self) => self.Load();

		private readonly AssetBundleIdent _ident;
		private readonly IAssetBundleSource _source;

		private AssetBundle _loadedAssetBundle;

		public AssetBundleFile(AssetBundleIdent ident, IAssetBundleSource source)
		{
			Guard.AgainstNull(ident, () => new ArgumentNullException(nameof(ident)));
			Guard.AgainstNull(source, () => new ArgumentNullException(nameof(source)));

			_ident = ident;
			_source = source;
		}

		public IAssetIdent Ident => _ident;
		public bool IsLoaded => _loadedAssetBundle != null;
		public FileSize Size { get; private set; } = FileSize.Unknown;

		public AssetBundle Load()
		{
			if (IsLoaded)
			{
				return _loadedAssetBundle;
			}

			var loadedAssetBundle = _source.Load(by: _ident.AbsolutePathWithoutExtension);
			Guard.AgainstNull(loadedAssetBundle, () => new AssetBundleNotLoaded(Ident.Uri));

			_loadedAssetBundle = loadedAssetBundle;
			Size = _source.Size(of: _loadedAssetBundle);

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

			var loadedAssetBundle = await _source.LoadAsync(by: _ident.AbsolutePathWithoutExtension,
				withProgress: progress => onProgress?.Invoke(new DownloadProgress(progress)),
				cancellationToken);

			Guard.AgainstNull(loadedAssetBundle, () => new AssetBundleNotLoaded(Ident.Uri));

			_loadedAssetBundle = loadedAssetBundle;
			onProgress?.Invoke(DownloadProgress.Full);
			Size = _source.Size(of: _loadedAssetBundle);

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