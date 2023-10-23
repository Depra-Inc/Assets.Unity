// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Threading;
using System.Threading.Tasks;
using Depra.Assets.Delegates;
using Depra.Assets.Files;
using Depra.Assets.Idents;
using Depra.Assets.Exceptions;
using Depra.Assets.Files.Resource.Exceptions;
using Depra.Assets.ValueObjects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Files.Resource
{
	public sealed class ResourcesAsset<TAsset> : ILoadableAsset<TAsset>, IDisposable where TAsset : Object
	{
		public static implicit operator TAsset(ResourcesAsset<TAsset> self) => self.Load();

		private readonly ResourcesPath _ident;
		private TAsset _loadedAsset;

		public ResourcesAsset(ResourcesPath ident)
		{
			Guard.AgainstNull(ident, () => new ArgumentNullException(nameof(ident)));

			_ident = ident;
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

			var loadedAsset = Resources.Load<TAsset>(_ident.RelativePath);
			Guard.AgainstNull(loadedAsset, () => new ResourceNotLoaded(_ident.RelativePath));

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

			Resources.UnloadAsset(_loadedAsset);
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

			var loadedAsset = await Resources.LoadAsync(_ident.RelativePath)
				.ToTask(progress => onProgress?.Invoke(new DownloadProgress(progress)),
					cancellationToken: cancellationToken);

			Guard.AgainstNull(loadedAsset, () => new ResourceNotLoaded(_ident.RelativePath));

			_loadedAsset = (TAsset) loadedAsset;
			onProgress?.Invoke(DownloadProgress.Full);
			Size = UnityFileSize.FromProfiler(_loadedAsset);

			return _loadedAsset;
		}

		void IDisposable.Dispose() => Unload();
	}
}