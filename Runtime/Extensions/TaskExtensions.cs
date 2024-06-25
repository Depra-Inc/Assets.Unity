// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Depra.Asset.Extensions
{
	internal static class TaskExtensions
	{
		public static IEnumerator ToCoroutine(this Task self, Action<Exception> exception = null) =>
			new ToCoroutineEnumerator(self, exception);

		public static void Forget(this Task _) { }

		private sealed record ToCoroutineEnumerator : IEnumerator
		{
			private readonly Task _task;
			private readonly Action<Exception> _exceptionHandler;

			private bool _isStarted;
			private bool _completed;
			private ExceptionDispatchInfo _exception;

			public ToCoroutineEnumerator(Task task, Action<Exception> exceptionHandler)
			{
				_task = task;
				_completed = false;
				_exceptionHandler = exceptionHandler;
			}

			private async Task RunTask(Task task)
			{
				try
				{
					await task;
				}
				catch (Exception exception)
				{
					if (_exceptionHandler != null)
					{
						_exceptionHandler(exception);
					}
					else
					{
						_exception = ExceptionDispatchInfo.Capture(exception);
					}
				}
				finally
				{
					_completed = true;
				}
			}

			object IEnumerator.Current => null;

			bool IEnumerator.MoveNext()
			{
				if (_isStarted == false)
				{
					_isStarted = true;
					RunTask(_task).Forget();
				}

				if (_exception == null)
				{
					return _completed == false;
				}

				_exception.Throw();
				return false;
			}

			void IEnumerator.Reset() { }
		}
	}

	internal static class ATask
	{
		public static void Void(Func<Task> taskFactory) => taskFactory().Forget();

		public static IEnumerator ToCoroutine(Func<Task> taskFactory, Action<Exception> exception = null) =>
			taskFactory().ToCoroutine(exception);
	}
}