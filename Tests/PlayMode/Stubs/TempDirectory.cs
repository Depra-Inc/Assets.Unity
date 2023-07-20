// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.IO;
using System.Linq;
using Depra.Assets.Unity.Runtime.Common;

namespace Depra.Assets.Unity.Tests.PlayMode.Stubs
{
    internal sealed class TempDirectory
    {
        private readonly DirectoryInfo _directoryInfo;

        public TempDirectory(string path)
        {
            _directoryInfo = new DirectoryInfo(path);
            CreateIfDoesNotExist();
        }

        public void DeleteIfEmpty()
        {
            if (_directoryInfo.Exists == false || IsEmpty() == false)
            {
                return;
            }
            
            File.Delete(_directoryInfo.FullName + AssetTypes.META);
            _directoryInfo.Delete(true);
        }

        private void CreateIfDoesNotExist()
        {
            if (_directoryInfo.Exists == false)
            {
                _directoryInfo.Create();
            }
        }

        private bool IsEmpty() => 
            _directoryInfo.EnumerateFileSystemInfos().Any() == false;
    }
}