using Depra.Assets.ValueObjects;
using UnityEngine;

namespace Depra.Assets.Unity.Runtime.Files.Bundles.Sources
{
    internal readonly struct AssetBundleWithSize
    {
        public readonly FileSize Size;
        public readonly AssetBundle AssetBundle;

        public AssetBundleWithSize(AssetBundle assetBundle, FileSize size)
        {
            Size = size;
            AssetBundle = assetBundle;
        }
    }
}