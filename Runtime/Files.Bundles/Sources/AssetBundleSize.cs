// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Depra.Asset.ValueObjects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Asset.Files.Bundles.Sources
{
	public static class AssetBundleSize
	{
		public static FileSize Evaluate(AssetBundle assetBundle)
		{
			FileSize fileSize;
#if UNITY_EDITOR
			fileSize = SizeInRAM(assetBundle);
			if (fileSize.Bytes == 0)
#endif
			{
				fileSize = SizeOnDisk(assetBundle);
			}

			return fileSize;
		}

		private static FileSize SizeOnDisk(AssetBundle assetBundle)
		{
			var allScenePaths = assetBundle.GetAllScenePaths();
			var sizes = from scenePath in allScenePaths
				select Path.Combine(Application.streamingAssetsPath, scenePath)
				into absolutePath
				select new FileInfo(absolutePath)
				into fileInfo
				where fileInfo.Exists
				select fileInfo.Length;

			return new FileSize(sizes.Sum());
		}

#if UNITY_EDITOR
		/// <summary>
		/// Returns size of <see cref="AssetBundle"/> in RAM.
		/// </summary>
		/// <param name="assetBundle"><see cref="AssetBundle"/> for calculating.</param>
		/// <returns></returns>
		/// <remarks>Source - https://stackoverflow.com/questions/56822948/estimate-an-assetbundle-size-in-ram</remarks>
		private static FileSize SizeInRAM(Object assetBundle)
		{
			var sizes = new Dictionary<Type, long>();
			var serializedObject = new UnityEditor.SerializedObject(assetBundle);
			var serializedProperty = serializedObject.FindProperty("m_PreloadTable");
			for (var index = 0; index < serializedProperty.arraySize; index++)
			{
				var objectReference = serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue;
				var type = objectReference.GetType();
				var size = UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(objectReference);
				if (sizes.TryAdd(type, size) == false)
				{
					sizes[type] += size;
				}
			}

			return new FileSize(sizes.Sum(x => x.Value));
		}
#endif
	}
}