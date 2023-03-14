using Depra.Assets.Runtime.Common;
using UnityEngine;

namespace Depra.Assets.Tests.PlayMode.Configuration
{
    [CreateAssetMenu(fileName = nameof(TestAssetDirectory),
        menuName = Constants.MODULE_NAME + "/" + nameof(TestAssetDirectory), order = 51)]
    internal sealed class TestAssetDirectory : ScriptableObject
    {
        [field: SerializeField] public string Path { get; private set; }
        [field: SerializeField] public string[] Assets { get; private set; }
        [field: SerializeField] public TestAssetDirectory[] Directories { get; private set; }
    }
}