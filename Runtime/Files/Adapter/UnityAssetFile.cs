// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Depra.Assets.Delegates;
using Depra.Assets.Files;
using Depra.Assets.Idents;
using Depra.Assets.ValueObjects;

namespace Depra.Assets.Unity.Runtime.Files.Adapter
{
	public abstract class UnityAssetFile<TAsset> : ILoadableAsset<TAsset>, IUnityLoadableAsset<TAsset>
	{
		public abstract bool IsLoaded { get; }

		public abstract IAssetIdent Ident { get; }

		public abstract FileSize Size { get; protected set; }

		public abstract TAsset Load();

		public abstract void Unload();

		public abstract UniTask<TAsset> LoadAsync(DownloadProgressDelegate onProgress = null,
			CancellationToken cancellationToken = default);

		Task<TAsset> ILoadableAsset<TAsset>.LoadAsync(DownloadProgressDelegate onProgress,
			CancellationToken cancellationToken) =>
			LoadAsync(onProgress, cancellationToken).AsTask();
	}
}