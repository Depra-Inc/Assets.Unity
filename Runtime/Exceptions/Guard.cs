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
		private const string CONDITION = "DEBUG";

		[Conditional(CONDITION)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Against(bool condition, Func<Exception> exception)
		{
			if (condition)
			{
				throw exception();
			}
		}

		[Conditional(CONDITION)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AgainstNull<TObject>(TObject asset, Func<Exception> exception) =>
			Against(asset == null, exception);

		[Conditional(CONDITION)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AgainstEqual<T>(IEquatable<T> value, IEquatable<T> other, Func<Exception> exception) =>
			Against(value.Equals(other), exception);

		[Conditional(CONDITION)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AgainstFileNotFound(string filePath) =>
			Against(File.Exists(filePath) == false, () => new FileNotFoundException(filePath));

		[Conditional(CONDITION)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AgainstEmptyString(string value, Func<Exception> exception) =>
			Against(string.IsNullOrEmpty(value), exception);
	}
}