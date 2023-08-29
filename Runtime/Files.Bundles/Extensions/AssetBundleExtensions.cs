// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Depra.Assets.ValueObjects;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEngine.Profiling;
using UnityEditor;
#endif

namespace Depra.Assets.Unity.Runtime.Files.Bundles.Extensions
{
	public static class AssetBundleExtensions
	{
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

		private static FileSize SizeOnDisk(this AssetBundle assetBundle)
		{
			var allScenePaths = assetBundle.GetAllScenePaths();
			var sizes = from scenePath in allScenePaths
				select Path.Combine(Application.streamingAssetsPath, scenePath)
				into absolutePath
				select new FileInfo(absolutePath)
				into fileInfo
				where fileInfo.Exists
				select fileInfo.Length;

			var sizeInBytes = sizes.Sum();

			return new FileSize(sizeInBytes);
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

			var sizeInBytes = sizes.Sum(x => x.Value);

			return new FileSize(sizeInBytes);
		}
#endif
	}
}