// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

namespace Depra.Assets.Unity.Runtime.Common
{
	internal static class Constants
	{
		public const string FRAMEWORK_NAME = "Depra";
		public const string ENGINE_NAME = "Unity";
		public const string MODULE_NAME = "Assets";

		internal static readonly string FullModuleName = string.Join('.', FRAMEWORK_NAME, MODULE_NAME);
	}
}