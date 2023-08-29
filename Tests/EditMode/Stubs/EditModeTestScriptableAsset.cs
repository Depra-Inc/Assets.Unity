// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using UnityEngine;
using static Depra.Assets.Unity.Runtime.Common.Constants;
using static Depra.Assets.Unity.Runtime.Common.Paths;

namespace Depra.Assets.Unity.Tests.EditMode.Stubs
{
	[CreateAssetMenu(fileName = nameof(EditModeTestScriptableAsset), menuName = MENU_PATH, order = 52)]
	internal sealed class EditModeTestScriptableAsset : ScriptableObject
	{
		private const string TESTS_FOLDER_NAME = nameof(Tests);

		private const string MENU_PATH = FRAMEWORK_NAME + SLASH +
		                                 MODULE_NAME + SLASH +
		                                 TESTS_FOLDER_NAME + SLASH +
		                                 nameof(EditMode) + SLASH +
		                                 nameof(EditModeTestScriptableAsset);
	}
}