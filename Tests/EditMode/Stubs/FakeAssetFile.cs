// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Threading;
using System.Threading.Tasks;
using Depra.Assets.Delegates;
using Depra.Assets.Files;
using Depra.Assets.ValueObjects;

namespace Depra.Assets.Tests.EditMode.Stubs
{
	internal sealed class FakeAssetFile : IAssetFile<EditModeTestScriptableAsset>
	{
		public FakeAssetFile(IAssetUri ident) => Metadata = new AssetMetadata(ident, FileSize.Zero);

		public AssetMetadata Metadata { get; }
		public bool IsLoaded { get; private set; }

		void IAssetFile.Unload() => IsLoaded = false;

		EditModeTestScriptableAsset IAssetFile<EditModeTestScriptableAsset>.Load() =>
			throw new NotImplementedException();

		Task<EditModeTestScriptableAsset> IAssetFile<EditModeTestScriptableAsset>.LoadAsync(
			DownloadProgressDelegate onProgress,
			CancellationToken cancellationToken) =>
			throw new NotImplementedException();
	}
}