using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Depra.Assets.Delegates;
using Depra.Assets.Files;
using Depra.Assets.ValueObjects;

namespace Depra.Assets.Tests.EditMode.Stubs
{
	internal sealed class FakeAssetFileWithDependency : IAssetFile<EditModeTestScriptableAsset>
	{
		public static readonly IAssetUri FAKE_DEPENDENCY = new AssetName(nameof(FAKE_DEPENDENCY));

		public FakeAssetFileWithDependency(IAssetUri ident) => Metadata = new AssetMetadata(ident, FileSize.Zero);

		public AssetMetadata Metadata { get; }
		public bool IsLoaded { get; private set; }

		void IAssetFile.Unload() => IsLoaded = false;

		IEnumerable<IAssetUri> IAssetFile.Dependencies() => new[] { FAKE_DEPENDENCY };

		EditModeTestScriptableAsset IAssetFile<EditModeTestScriptableAsset>.Load() =>
			throw new NotImplementedException();

		Task<EditModeTestScriptableAsset> IAssetFile<EditModeTestScriptableAsset>.LoadAsync(
			DownloadProgressDelegate onProgress,
			CancellationToken cancellationToken) =>
			throw new NotImplementedException();
	}
}