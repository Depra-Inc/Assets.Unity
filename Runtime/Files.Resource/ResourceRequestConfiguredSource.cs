using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using Depra.Assets.Delegates;
using Depra.Assets.Runtime.Utility;
using Depra.Assets.ValueObjects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Files.Resource
{
	public sealed record ResourceRequestConfiguredSource : IValueTaskSource<Object>
	{
		public static ValueTask<Object> Create(ResourceRequest request, DownloadProgressDelegate progress,
			CancellationToken cancellationToken, out short token)
		{
			var source = new ResourceRequestConfiguredSource(request, progress, cancellationToken);
			token = source._token;

			return new ValueTask<Object>(source, token);
		}

		private readonly ResourceRequest _asyncOperation;
		private readonly DownloadProgressDelegate _progress;
		private readonly CancellationToken _cancellationToken;

		private short _token;
		private object _state;
		private Action<object> _continuation;

		private ResourceRequestConfiguredSource(ResourceRequest asyncOperation,
			DownloadProgressDelegate progress, CancellationToken cancellationToken = default)
		{
			_progress = progress;
			_asyncOperation = asyncOperation;
			_cancellationToken = cancellationToken;

			TaskTracker.AddUpdateFunction(Execute);
			cancellationToken.Register(Cancel);
		}

		private void Execute()
		{
			if (_cancellationToken.IsCancellationRequested)
			{
				TrySetCanceled();
				return;
			}

			if (_asyncOperation.isDone)
			{
				TrySetResult();
			}
			else
			{
				_progress?.Invoke(new DownloadProgress(_asyncOperation.progress));
			}
		}

		private void Cancel() => TrySetCanceled();

		private void TrySetResult()
		{
			TaskTracker.RemoveUpdateFunction(Execute);
			_continuation?.Invoke(_state);
		}

		private void TrySetCanceled()
		{
			TaskTracker.RemoveUpdateFunction(Execute);
			_continuation?.Invoke(_state);
		}

		void IValueTaskSource<Object>.OnCompleted(Action<object> continuation, object state, short token,
			ValueTaskSourceOnCompletedFlags flags)
		{
			_state = state;
			_continuation = continuation;
		}

		Object IValueTaskSource<Object>.GetResult(short token) => _asyncOperation.asset;

		ValueTaskSourceStatus IValueTaskSource<Object>.GetStatus(short token) => _asyncOperation.isDone
			? ValueTaskSourceStatus.Succeeded
			: ValueTaskSourceStatus.Pending;
	}
}