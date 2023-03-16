using System.IO;
using UnityEngine;
using static Depra.Assets.Runtime.Common.Constants;

namespace Depra.Assets.Tests.PlayMode.Types
{
    [CreateAssetMenu(menuName = FRAMEWORK_NAME + "/" + MODULE_NAME + "/" + nameof(TestResourcesRef), order = 51)]
    internal sealed class TestResourcesRef : ScriptableObject
    {
        private const string RESOURCES_FOLDER_NAME = "Resources";

        [field: SerializeField] public string AssetName { get; private set; } = "TestAsset";
        [field: SerializeField] public string DirectoryPath { get; private set; }

        public string AbsoluteDirectoryPath => Path.Combine(RESOURCES_FOLDER_NAME, DirectoryPath);
    }
}