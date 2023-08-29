// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using UnityEngine;
using static Depra.Assets.Runtime.Common.Module;
using static Depra.Assets.Runtime.Common.Paths;

namespace Depra.Assets.Unity.Tests.EditMode.Stubs
{
	[CreateAssetMenu(fileName = FILE_NAME, menuName = MENU_PATH, order = DEFAULT_ORDER)]
	internal sealed class EditModeTestScriptableAsset : ScriptableObject
	{
		private const string FILE_NAME = nameof(EditModeTestScriptableAsset);
		private const string MENU_PATH = FRAMEWORK_NAME + SLASH +
		                                 MODULE_NAME + SLASH +
		                                 nameof(Tests) + SLASH +
		                                 nameof(EditMode) + SLASH + FILE_NAME;
	}
}