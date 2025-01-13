// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Depra.Assets.Exceptions;
using Depra.Assets.Files.Resource.Exceptions;
using Depra.Threading;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Files.Resource
{
	public sealed class ResourcesAsset<TAsset> : IAssetFile<TAsset>, IDisposable where TAsset : Object
	{
		private readonly ResourcesPath _path;
		private TAsset _loadedAsset;

		public ResourcesAsset(ResourcesPath path)
		{
			Guard.AgainstNull(path, nameof(path));
			Metadata = new AssetMetadata(_path = path, FileSize.Unknown);
		}

		public AssetMetadata Metadata { get; }
		public bool IsLoaded => _loadedAsset != null;

		public TAsset Load()
		{
			if (IsLoaded)
			{
				return _loadedAsset;
			}

			var loadedAsset = Resources.Load<TAsset>(Metadata.Uri.Relative);
			Guard.AgainstNull(loadedAsset, () => new ResourceNotLoaded(Metadata.Uri.Relative));

			_loadedAsset = loadedAsset;
			Metadata.Size = UnityFileSize.FromProfiler(_loadedAsset);

			return _loadedAsset;
		}

		public void Unload()
		{
			if (IsLoaded == false || typeof(TAsset).IsSubclassOf(typeof(MonoBehaviour)))
			{
				return;
			}

			Resources.UnloadAsset(_loadedAsset);
			_loadedAsset = null;
		}

		public async ITask<TAsset> LoadAsync(DownloadProgressDelegate onProgress = null,
			CancellationToken cancellationToken = default)
		{
			if (IsLoaded)
			{
				onProgress?.Invoke(DownloadProgress.Full);
				return _loadedAsset;
			}

			var loadedAsset = await Resources
				.LoadAsync<TAsset>(Metadata.Uri.Relative)
				.ToTask(OnProgress, cancellationToken);

			Guard.AgainstNull(loadedAsset, () => new ResourceNotLoaded(Metadata.Uri.Relative));

			_loadedAsset = (TAsset) loadedAsset;
			onProgress?.Invoke(DownloadProgress.Full);
			Metadata.Size = UnityFileSize.FromProfiler(_loadedAsset);

			return _loadedAsset;

			void OnProgress(float progress) => onProgress?.Invoke(new DownloadProgress(progress));
		}

		public IEnumerable<IAssetUri> Dependencies() =>
#if UNITY_EDITOR
			UnityEditor.AssetDatabase
				.GetDependencies(_path.Project, recursive: false)
				.Select(path => new AssetName(path));
#else
			Array.Empty<IAssetUri>();
#endif

		void IDisposable.Dispose() => Unload();
	}
}