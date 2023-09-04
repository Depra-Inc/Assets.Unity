// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Assets.Idents;

namespace Depra.Assets.Tests.EditMode.Stubs
{
	internal sealed record FakeAssetIdent : IAssetIdent
	{
		public FakeAssetIdent(string name)
		{
			Uri = name;
			RelativeUri = name;
		}

		public string Uri { get; }
		public string RelativeUri { get; }
	}
}