using UnityEngine;
using static Depra.Assets.Unity.Runtime.Common.Constants;
using static Tests.EditMode.StaticData;

namespace Depra.Assets.Unity.Tests.EditMode.Stubs
{
    [CreateAssetMenu(fileName = nameof(EditModeTestScriptableAsset), menuName = MENU_PATH, order = 52)]
    internal sealed class EditModeTestScriptableAsset : ScriptableObject
    {
        private const string MENU_PATH = FRAMEWORK_NAME + "/" +
                                         MODULE_NAME + "/" +
                                         TESTS_FOLDER_NAME + "/" +
                                         nameof(EditMode) + "/" +
                                         nameof(EditModeTestScriptableAsset);
    }
}