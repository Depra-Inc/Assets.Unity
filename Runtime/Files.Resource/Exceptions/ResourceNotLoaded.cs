// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.Assets.Runtime.Files.Resource.Exceptions
{
	internal sealed class ResourceNotLoaded : Exception
	{
		private const string MESSAGE_FORMAT = "Resource was not loaded by path: {0}!";

		public ResourceNotLoaded(string assetPath) : base(string.Format(MESSAGE_FORMAT, assetPath)) { }
	}
}