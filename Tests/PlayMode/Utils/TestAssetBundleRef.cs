using UnityEngine;
using static Depra.Assets.Runtime.Common.Constants;

namespace Depra.Assets.Tests.PlayMode.Utils
{
    [CreateAssetMenu(menuName = FRAMEWORK_NAME + "/" + MODULE_NAME + "/" + nameof(TestAssetBundleRef), order = 51)]
    internal sealed class TestAssetBundleRef : ScriptableObject
    {
        [field: SerializeField] public string Path { get; private set; }
        [field: SerializeField] public string[] Assets { get; private set; }
        [field: SerializeField] public string BundleName { get; private set; }

        public string AbsoluteDirectoryPath => System.IO.Path.Combine(Application.streamingAssetsPath, Path);
    }
}