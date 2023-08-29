// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using UnityEngine;
using static Depra.Assets.Unity.Runtime.Common.Constants;
using static Depra.Assets.Unity.Runtime.Common.Paths;

namespace Depra.Assets.Unity.Tests.PlayMode.Stubs
{
	[CreateAssetMenu(fileName = nameof(PlayModeTestScriptableAsset), menuName = MENU_PATH, order = 52)]
	internal sealed class PlayModeTestScriptableAsset : ScriptableObject
	{
		private const string TESTS_FOLDER = nameof(Tests);

		private const string MENU_PATH = FRAMEWORK_NAME + SLASH +
		                                 MODULE_NAME + SLASH +
		                                 TESTS_FOLDER + SLASH +
		                                 nameof(PlayMode) + SLASH +
		                                 nameof(PlayModeTestScriptableAsset);
	}
}