using System.Collections.Generic;
using Depra.Assets.Runtime.Files.Interfaces;
using UnityEngine;

namespace Depra.Assets.Runtime.Files.Bundles.Extensions
{
    public static class AssetBundleFileExtensions
    {
        public static IEnumerable<string> AllAssetNames(this ILoadableAsset<AssetBundle> self) =>
            self.Load().GetAllAssetNames();
    }
}