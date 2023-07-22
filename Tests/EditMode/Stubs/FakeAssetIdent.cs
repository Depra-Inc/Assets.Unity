// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using Depra.Assets.Idents;

namespace Depra.Assets.Unity.Tests.EditMode.Stubs
{
    internal sealed class FakeAssetIdent : IAssetIdent
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