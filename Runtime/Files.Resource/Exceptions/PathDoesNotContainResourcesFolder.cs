// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;

namespace Depra.Assets.Runtime.Files.Resource.Exceptions
{
	internal sealed class PathDoesNotContainResourcesFolder : Exception
	{
		private const string MESSAGE_FORMAT = "The specified path {0} does not contain the Resources folder!";

		public PathDoesNotContainResourcesFolder(string path) : base(string.Format(MESSAGE_FORMAT, path)) { }
	}
}