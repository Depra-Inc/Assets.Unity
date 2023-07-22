using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Depra.Assets.Unity.Runtime.Common;

namespace Depra.Assets.Unity.Runtime.Extensions
{
    public static class DirectoryInfoExtensions
    {
        public static bool IsEmpty(this DirectoryInfo directoryInfo) =>
            directoryInfo.EnumerateFileSystemInfos().Any() == false;

        public static void DeleteIfEmpty(this DirectoryInfo directoryInfo)
        {
            if (directoryInfo.Exists == false || directoryInfo.IsEmpty() == false)
            {
                return;
            }

            directoryInfo.Delete(true);
            File.Delete(directoryInfo.FullName + AssetTypes.META);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DirectoryInfo CreateIfNotExists(this DirectoryInfo directoryInfo)
        {
            if (directoryInfo.Exists == false)
            {
                directoryInfo.Create();
            }

            return directoryInfo;
        }
    }
}