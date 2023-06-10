using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Depra.Assets.Runtime.Extensions
{
    public static class DirectoryInfoExtensions
    {
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