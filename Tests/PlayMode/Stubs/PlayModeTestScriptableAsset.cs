// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using UnityEngine;
using static Depra.Assets.Runtime.Common.Module;
using static Depra.Assets.Runtime.Common.Paths;

namespace Depra.Assets.Tests.PlayMode.Stubs
{
	[CreateAssetMenu(fileName = FILE_NAME, menuName = MENU_PATH, order = DEFAULT_ORDER)]
	internal sealed class PlayModeTestScriptableAsset : ScriptableObject
	{
		private const string FILE_NAME = nameof(PlayModeTestScriptableAsset);
		private const string MENU_PATH = FRAMEWORK_NAME + SLASH +
		                                 MODULE_NAME + SLASH +
		                                 nameof(Tests) + SLASH +
		                                 nameof(PlayMode) + SLASH + FILE_NAME;
	}
}