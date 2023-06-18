using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Files.Bundles.Web;
using UnityEngine.Networking;

namespace Depra.Assets.Runtime.Exceptions
{
    internal static class Guard
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AgainstNull<TObject>(TObject asset, Func<Exception> exceptionFunc)
        {
            if (asset == null)
            {
                throw exceptionFunc();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AgainstInvalidRequestResult(UnityWebRequest request,
            Func<string, string, Exception> exceptionFunc)
        {
            if (request.CanGetResult() == false)
            {
                throw exceptionFunc(request.error, request.url);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AgainstEqual<T>(IEquatable<T> value, IEquatable<T> other, Func<Exception> exceptionFunc)
        {
            if (value.Equals(other))
            {
                throw exceptionFunc();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AgainstAlreadyContains<T>(T element, List<T> @in, Func<Exception> exceptionFunc)
        {
            if (@in.Contains(element))
            {
                throw exceptionFunc();
            }
        }
    }
}