// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

namespace Depra.Assets.Internal
{
	internal static class Module
	{
		public const string MODULE_NAME = nameof(Assets);
		public const string FRAMEWORK_NAME = nameof(Depra);

		internal const int DEFAULT_ORDER = 52;
		internal static readonly string FULL_MODULE_NAME = string.Join('.', FRAMEWORK_NAME, MODULE_NAME);
	}
}