// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.IO;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Files.Bundles.Sources;
using UnityEngine.Networking;

namespace Depra.Assets.Runtime.Exceptions
{
	internal static class Guard
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AgainstNull<TObject>(TObject asset, Func<Exception> exceptionFactory)
		{
			if (asset == null)
			{
				throw exceptionFactory();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AgainstInvalidRequestResult(UnityWebRequest request,
			Func<string, string, Exception> exceptionFactory)
		{
			if (request.CanGetResult() == false)
			{
				throw exceptionFactory(request.error, request.url);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AgainstEqual<T>(IEquatable<T> value, IEquatable<T> other, Func<Exception> exceptionFactory)
		{
			if (value.Equals(other))
			{
				throw exceptionFactory();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AgainstFileNotFound(string filePath)
		{
			if (File.Exists(filePath) == false)
			{
				throw new FileNotFoundException(filePath);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AgainstEmptyString(string value, Func<Exception> exceptionFactory)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw exceptionFactory();
			}
		}
	}
}