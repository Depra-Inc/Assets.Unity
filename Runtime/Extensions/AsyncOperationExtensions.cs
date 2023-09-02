using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Depra.Assets.Runtime.Extensions
{
	public static class AsyncOperationExtensions
	{
		public static TaskAwaiter<T> GetAwaiter<T>(this AsyncOperation self)
		{
			var tcs = new TaskCompletionSource<T>();
			self.completed += _ => { tcs.SetResult(default); };
			var awaiter = tcs.Task.GetAwaiter();

			return awaiter;
		}
	}
}