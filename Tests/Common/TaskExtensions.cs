using System;
using System.Collections;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Depra.Assets.Tests.Common
{
    internal static class TaskExtensions
    {
        public static IEnumerator AsIEnumeratorReturnNull<T>(this Task<T> task)
        {
            while (task.IsCompleted == false)
            {
                yield return null;
            }
 
            if (task.IsFaulted)
            {
                ExceptionDispatchInfo.Capture(task.Exception!).Throw();
            }
 
            yield return null;
        }
        
        public static T RunAsyncMethodSync<T>(this Func<Task<T>> asyncFunc) =>
            Task.Run(async () => await asyncFunc()).GetAwaiter().GetResult();

        public static void RunAsyncMethodSync(this Func<Task> asyncFunc) =>
            Task.Run(async () => await asyncFunc()).GetAwaiter().GetResult();
    }
}