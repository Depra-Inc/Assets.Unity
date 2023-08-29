// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.IO;
using System.Runtime.CompilerServices;
using Depra.Assets.Unity.Runtime.Files.Bundles.Sources;
using UnityEngine.Networking;

namespace Depra.Assets.Unity.Runtime.Exceptions
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
		public static void AgainstFileNotFound(string filePath)
		{
			if (File.Exists(filePath) == false)
			{
				throw new FileNotFoundException(filePath);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AgainstEmptyString(string value, Func<Exception> exceptionFunc)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw exceptionFunc();
			}
		}
	}
}