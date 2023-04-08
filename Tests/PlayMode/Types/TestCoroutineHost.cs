// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.Collections;
using Depra.Coroutines.Domain.Entities;
using Depra.Coroutines.Unity.Runtime;
using UnityEngine;

namespace Depra.Assets.Tests.PlayMode.Types
{
    internal sealed class TestCoroutineHost : MonoBehaviour, ICoroutineHost
    {
        private RuntimeCoroutineHost _coroutineHost;

        public static TestCoroutineHost Create() =>
            new GameObject().AddComponent<TestCoroutineHost>();

        private void Awake() => 
            _coroutineHost = gameObject.AddComponent<RuntimeCoroutineHost>();

        public new ICoroutine StartCoroutine(IEnumerator process) =>
            _coroutineHost.StartCoroutine(process);

        public void StopCoroutine(ICoroutine coroutine) => 
            _coroutineHost.StopCoroutine(coroutine);
    }
}