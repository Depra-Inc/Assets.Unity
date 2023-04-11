// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.Collections;
using Depra.Coroutines.Domain.Entities;
using Depra.Coroutines.Unity.Runtime;
using UnityEngine;

namespace Depra.Assets.Tests.PlayMode.Mocks
{
    internal sealed class CoroutineHostMock : MonoBehaviour, ICoroutineHost
    {
        private RuntimeCoroutineHost _coroutineHost;

        public static CoroutineHostMock Create() =>
            new GameObject().AddComponent<CoroutineHostMock>();

        private void Awake() => 
            _coroutineHost = gameObject.AddComponent<RuntimeCoroutineHost>();

        public new ICoroutine StartCoroutine(IEnumerator process) =>
            _coroutineHost.StartCoroutine(process);

        public void StopCoroutine(ICoroutine coroutine) => 
            _coroutineHost.StopCoroutine(coroutine);
    }
}