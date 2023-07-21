// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using UnityEngine;
using static Depra.Assets.Unity.Runtime.Common.Constants;

namespace Depra.Assets.Unity.Tests.PlayMode.Stubs
{
    [CreateAssetMenu(fileName = nameof(PlayModeTestScriptableAsset), menuName = MENU_PATH, order = 52)]
    internal sealed class PlayModeTestScriptableAsset : ScriptableObject
    {
        private const string TESTS_FOLDER = "Tests";

        private const string MENU_PATH = FRAMEWORK_NAME + "/" +
                                         MODULE_NAME + "/" +
                                         TESTS_FOLDER + "/" +
                                         nameof(PlayMode) + "/" +
                                         nameof(PlayModeTestScriptableAsset);
    }
}