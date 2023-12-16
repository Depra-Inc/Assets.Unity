using System.Runtime.CompilerServices;

namespace Depra.Assets.Extensions
{
	public static class StringExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToUnixPath(this string path) => path.Replace('\\', '/');
	}
}