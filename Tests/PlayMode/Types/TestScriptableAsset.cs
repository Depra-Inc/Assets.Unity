// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using UnityEngine;
using static Depra.Assets.Runtime.Common.Constants;

namespace Depra.Assets.Tests.PlayMode.Types
{
    [CreateAssetMenu(menuName = FrameworkName + "/" + ModuleName + "/" + nameof(TestScriptableAsset), order = 51)]
    public sealed class TestScriptableAsset : ScriptableObject { }
}