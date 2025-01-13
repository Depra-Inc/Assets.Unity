// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Depra.Assets.Exceptions;
using Depra.Threading;
using UnityEngine;

namespace Depra.Assets.Files.Database
{
	public sealed class RuntimeDatabaseAsset<TAsset> : IAssetFile<TAsset>, IDisposable where TAsset : ScriptableObject
	{
		private TAsset _loadedAsset;

		public RuntimeDatabaseAsset(string name) : this((IAssetUri) new AssetName(name)) { }

		public RuntimeDatabaseAsset(IAssetUri uri) => Metadata = new AssetMetadata(uri, FileSize.Unknown);

		public AssetMetadata Metadata { get; }
		public bool IsLoaded => _loadedAsset != null;

		public TAsset Load()
		{
			var loadedAsset = ScriptableObject.CreateInstance<TAsset>();
			Guard.AgainstNull(loadedAsset, () => new AssetCatNotBeCreated(typeof(TAsset), nameof(TAsset)));

			return _loadedAsset = loadedAsset;
		}

		public void Unload()
		{
			if (_loadedAsset)
			{
				_loadedAsset = null;
			}
		}

		void IDisposable.Dispose() => Unload();

		IEnumerable<IAssetUri> IAssetFile.Dependencies() => Array.Empty<IAssetUri>();

		[Obsolete("Not yet supported in Unity. Use RuntimeDatabaseAsset<TAsset>.Load() instead")]
		ITask<TAsset> IAssetFile<TAsset>.LoadAsync(DownloadProgressDelegate onProgress,
			CancellationToken cancellationToken)
		{
			onProgress?.Invoke(DownloadProgress.Full);
			return Task.FromResult(IsLoaded ? _loadedAsset : Load()).AsITask();
		}
	}
}