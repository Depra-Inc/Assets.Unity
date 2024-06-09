// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Assets.ValueObjects;

namespace Depra.Assets.Tests.EditMode.Stubs
{
	internal sealed record FakeAssetUri : IAssetUri
	{
		public FakeAssetUri(string name) => Relative = Absolute = name;

		public string Relative { get; }
		public string Absolute { get; }
	}
}