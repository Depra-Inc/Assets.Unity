// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Files.Database;

namespace Depra.Assets.Tests.PlayMode.Utils
{
    internal sealed class TempDirectory : IDisposable
    {
        private readonly DirectoryInfo _directoryInfo;

        public TempDirectory(string path)
        {
            _directoryInfo = new DirectoryInfo(path);
            CreateIfDoesNotExist();
        }

        public void Dispose()
        {
            if (_directoryInfo.Exists == false || IsEmpty() == false)
            {
                return;
            }
            
            File.Delete(_directoryInfo.FullName + AssetTypes.META);
            _directoryInfo.Delete(true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateIfDoesNotExist()
        {
            if (_directoryInfo.Exists == false)
            {
                _directoryInfo.Create();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsEmpty() => 
            _directoryInfo.EnumerateFileSystemInfos().Any() == false;
    }
}