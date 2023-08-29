// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Delegates;
using Depra.Assets.Files;
using Depra.Assets.Idents;
using Depra.Assets.Runtime.Files.Adapter;
using Depra.Assets.ValueObjects;

namespace Depra.Assets.Unity.Tests.EditMode.Stubs
{
	internal sealed class FakeAssetFile : IUnityLoadableAsset<EditModeTestScriptableAsset>
	{
		public FakeAssetFile(IAssetIdent ident) => Ident = ident;

		public IAssetIdent Ident { get; }

		public bool IsLoaded { get; private set; }

		FileSize IAssetFile.Size => FileSize.Zero;

		EditModeTestScriptableAsset IUnityLoadableAsset<EditModeTestScriptableAsset>.Load() =>
			throw new NotImplementedException();

		void IUnityLoadableAsset<EditModeTestScriptableAsset>.Unload() => IsLoaded = false;

		UniTask<EditModeTestScriptableAsset> IUnityLoadableAsset<EditModeTestScriptableAsset>.LoadAsync(
			DownloadProgressDelegate onProgress,
			CancellationToken cancellationToken) =>
			throw new NotImplementedException();
	}
}