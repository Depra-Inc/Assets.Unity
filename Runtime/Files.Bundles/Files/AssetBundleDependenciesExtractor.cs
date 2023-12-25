using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Depra.Assets.Files.Bundles.Exceptions;
using Depra.Assets.ValueObjects;
using UnityEngine;

namespace Depra.Assets.Files.Bundles
{
	internal static class AssetBundleDependenciesExtractor
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<IAssetUri> Extract(AssetBundle assetBundle)
		{
			const string MANIFEST_NAME = "assetbundlemanifest";
			var manifest = assetBundle.LoadAsset<AssetBundleManifest>(MANIFEST_NAME);
			if (manifest == null)
			{
				throw new AssetBundleFileNotLoaded(MANIFEST_NAME, assetBundle.name);
			}

			var dependencies = manifest.GetAllDependencies(assetBundle.name);
			foreach (var dependencyPath in dependencies)
			{
				yield return new AssetBundleUri(dependencyPath);
			}
		}
	}
}