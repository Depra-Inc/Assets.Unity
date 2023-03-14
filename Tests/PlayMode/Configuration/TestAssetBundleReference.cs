using System.Linq;
using Depra.Assets.Runtime.Common;
using UnityEngine;

namespace Depra.Assets.Tests.PlayMode.Configuration
{
    [CreateAssetMenu(fileName = nameof(TestAssetBundleReference),
        menuName = Constants.MODULE_NAME + "/" + nameof(TestAssetBundleReference), order = 51)]
    public sealed class TestAssetBundleReference : ScriptableObject
    {
        [field: SerializeField] public string Path { get; private set; }
        [field: SerializeField] public string[] Assets { get; private set; }
        [field: SerializeField] public string BundleName { get; private set; }

        public static TestAssetBundleReference Load() =>
            Resources.FindObjectsOfTypeAll<TestAssetBundleReference>()
                .First() ?? throw new AssetsTestException(nameof(TestAssetBundleReference));

        public string AbsoluteDirectoryPath => System.IO.Path.Combine(Application.streamingAssetsPath, Path);
    }
}