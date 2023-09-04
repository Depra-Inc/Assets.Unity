// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

namespace Depra.Assets.Runtime.Common
{
	internal static class Module
	{
		public const string MODULE_NAME = nameof(Assets);
		public const string FRAMEWORK_NAME = nameof(Depra);
		public const string ENGINE_NAME = nameof(UnityEngine);

		internal const int DEFAULT_ORDER = 52;

		internal static readonly string FullModuleName = string.Join('.', FRAMEWORK_NAME, MODULE_NAME);
	}
}