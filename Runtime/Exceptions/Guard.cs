// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Depra.Assets.Exceptions
{
	internal static class Guard
	{
		[Conditional(Conditional.ENSURE)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Against(bool condition, Func<Exception> exception)
		{
			if (condition)
			{
				throw exception();
			}
		}

		[Conditional(Conditional.ENSURE)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AgainstNull<TObject>(TObject asset, Func<Exception> exception) =>
			Against(asset == null, exception);

		[Conditional(Conditional.ENSURE)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AgainstNull<TObject>(TObject asset, string argumentName) =>
			Against(asset == null, () => new ArgumentNullException(argumentName));

		[Conditional(Conditional.ENSURE)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AgainstEqual<T>(IEquatable<T> value, IEquatable<T> other, Func<Exception> exception) =>
			Against(value.Equals(other), exception);

		[Conditional(Conditional.ENSURE)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AgainstFileNotFound(string filePath) =>
			Against(File.Exists(filePath) == false, () => new FileNotFoundException(filePath));

		[Conditional(Conditional.ENSURE)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AgainstEmptyString(string value, Func<Exception> exception) =>
			Against(string.IsNullOrEmpty(value), exception);

		private static class Conditional
		{
			private const string TRUE = "DEBUG";
			private const string FALSE = "THIS_IS_JUST_SOME_RANDOM_STRING_THAT_IS_NEVER_DEFINED";

#if DEBUG || DEV_BUILD
			public const string ENSURE = TRUE;
#else
			public const string ENSURE = FALSE;
#endif
		}
	}
}