// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Delegates;
using Depra.Assets.Idents;
using Depra.Assets.Unity.Runtime.Exceptions;
using Depra.Assets.Unity.Runtime.Files.Adapter;
using Depra.Assets.Unity.Runtime.Files.Resource.Exceptions;
using Depra.Assets.ValueObjects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Unity.Runtime.Files.Resource
{
	public sealed class ResourceAsset<TAsset> : UnityAssetFile<TAsset>, IDisposable where TAsset : Object
	{
		public static implicit operator TAsset(ResourceAsset<TAsset> from) => from.Load();

		private readonly ResourcesPath _ident;
		private TAsset _loadedAsset;

		public ResourceAsset(ResourcesPath ident) =>
			_ident = ident ?? throw new ArgumentNullException(nameof(ident));

		public override IAssetIdent Ident => _ident;
		public override bool IsLoaded => _loadedAsset != null;
		public override FileSize Size { get; protected set; } = FileSize.Unknown;

		public override TAsset Load()
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

		public override void Unload()
		{
			if (IsLoaded == false)
			{
				return;
			}

			Resources.UnloadAsset(_loadedAsset);
			_loadedAsset = null;
		}

		public override async UniTask<TAsset> LoadAsync(DownloadProgressDelegate onProgress = null,
			CancellationToken cancellationToken = default)
		{
			if (IsLoaded)
			{
				onProgress?.Invoke(DownloadProgress.Full);
				return _loadedAsset;
			}

			var progress = Progress.Create<float>(value => onProgress?.Invoke(new DownloadProgress(value)));
			var loadedAsset = await Resources.LoadAsync<TAsset>(_ident.RelativePath)
				.ToUniTask(progress, cancellationToken: cancellationToken);

			Guard.AgainstNull(loadedAsset, () => new ResourceNotLoaded(_ident.RelativePath));

			_loadedAsset = (TAsset) loadedAsset;
			onProgress?.Invoke(DownloadProgress.Full);
			Size = UnityFileSize.FromProfiler(_loadedAsset);

			return _loadedAsset;
		}

		void IDisposable.Dispose() => Unload();
	}
}