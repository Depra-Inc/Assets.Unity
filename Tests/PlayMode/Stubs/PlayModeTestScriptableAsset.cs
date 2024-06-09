// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEngine;
using static Depra.Assets.Common.Module;
using static Depra.Assets.Common.UnityProject;

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