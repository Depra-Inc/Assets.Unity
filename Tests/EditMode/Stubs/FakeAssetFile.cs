// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Threading;
using System.Threading.Tasks;
using Depra.Assets.Delegates;
using Depra.Assets.Files;
using Depra.Assets.Idents;
using Depra.Assets.ValueObjects;

namespace Depra.Assets.Tests.EditMode.Stubs
{
	internal sealed class FakeAssetFile : ILoadableAsset<EditModeTestScriptableAsset>
	{
		public FakeAssetFile(IAssetIdent ident) => Ident = ident;

		public IAssetIdent Ident { get; }

		public bool IsLoaded { get; private set; }

		FileSize IAssetFile.Size => FileSize.Zero;

		EditModeTestScriptableAsset ILoadableAsset<EditModeTestScriptableAsset>.Load() =>
			throw new NotImplementedException();

		void ILoadableAsset<EditModeTestScriptableAsset>.Unload() => IsLoaded = false;

		Task<EditModeTestScriptableAsset> ILoadableAsset<EditModeTestScriptableAsset>.LoadAsync(
			DownloadProgressDelegate onProgress,
			CancellationToken cancellationToken) =>
			throw new NotImplementedException();
	}
}