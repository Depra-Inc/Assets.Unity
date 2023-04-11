// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Files.Structs;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEngine.Profiling;
using UnityEditor;
#endif

namespace Depra.Assets.Runtime.Files.Bundles.Extensions
{
    public static class AssetBundleExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("ReSharper", "JoinDeclarationAndInitializer")]
        public static FileSize Size(this AssetBundle assetBundle)
        {
            FileSize fileSize;
#if UNITY_EDITOR
            fileSize = SizeInRAM(assetBundle);
            if (fileSize.SizeInBytes == 0)
#endif
            {
                fileSize = SizeOnDisk(assetBundle);
            }

            return fileSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static FileSize SizeOnDisk(this AssetBundle assetBundle)
        {
            long bytes = 0;
            var allScenePaths = assetBundle.GetAllScenePaths();
            foreach (var scenePath in allScenePaths)
            {
                var absolutePath = Path.Combine(Application.streamingAssetsPath, scenePath);
                var fileInfo = new FileInfo(absolutePath);
                if (fileInfo.Exists)
                {
                    bytes += fileInfo.Length;
                }
            }

            return new FileSize(bytes);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Returns size of <see cref="AssetBundle"/> in RAM.
        /// </summary>
        /// <param name="assetBundle"><see cref="AssetBundle"/> for calculating.</param>
        /// <returns></returns>
        /// <remarks>Source - https://stackoverflow.com/questions/56822948/estimate-an-assetbundle-size-in-ram</remarks>
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static FileSize SizeInRAM(this Object assetBundle)
        {
            var serializedObject = new SerializedObject(assetBundle);
            var serializedProperty = serializedObject.FindProperty("m_PreloadTable");
            var sizes = new Dictionary<Type, long>();
            for (var i = 0; i < serializedProperty.arraySize; i++)
            {
                var objectReference = serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue;
                var type = objectReference.GetType();
                var size = Profiler.GetRuntimeMemorySizeLong(objectReference);
                if (sizes.ContainsKey(type))
                {
                    sizes[type] += size;
                }
                else
                {
                    sizes.Add(type, size);
                }
            }
            
            return new FileSize(sizes.Sum(x => x.Value));
        }
#endif
    }
}