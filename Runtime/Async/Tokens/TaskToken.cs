// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Depra.Assets.Runtime.Async.Tokens
{
    internal sealed class TaskToken : IAsyncToken
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        public TaskToken(Task task, CancellationTokenSource cancellationTokenSource)
        {
            Task = task;
            _cancellationTokenSource = cancellationTokenSource;
        }

        public bool IsCanceled { get; private set; }

        public void Cancel()
        {
            if (IsCanceled)
            {
                throw new OperationCanceledException();
            }
            
            IsCanceled = true;
            _cancellationTokenSource.Cancel();
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public Task Task { get; }
    }
}