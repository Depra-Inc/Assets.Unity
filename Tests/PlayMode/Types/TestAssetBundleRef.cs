// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.Linq;
using Depra.Assets.Tests.PlayMode.Exceptions;
using UnityEngine;
using static Depra.Assets.Runtime.Common.Constants;

namespace Depra.Assets.Tests.PlayMode.Types
{
    [CreateAssetMenu(menuName = FRAMEWORK_NAME + "/" + MODULE_NAME + "/" + nameof(TestAssetBundleRef), order = 51)]
    internal sealed class TestAssetBundleRef : ScriptableObject
    {
        [field: SerializeField] public string Path { get; private set; }
        [field: SerializeField] public string BundleName { get; private set; }
        [field: SerializeField] public string SingleAssetName { get; private set; }

        public static TestAssetBundleRef Load() =>
            Resources.LoadAll<TestAssetBundleRef>(string.Empty).FirstOrDefault() ??
            throw new TestReferenceNotFoundException(nameof(TestAssetBundleRef));

        public string AbsolutePath => System.IO.Path.Combine(AbsoluteDirectoryPath, BundleName);
        
        public string AbsoluteDirectoryPath => System.IO.Path.Combine(Application.streamingAssetsPath, Path);
    }
}