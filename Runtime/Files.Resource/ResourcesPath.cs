// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.IO;
using System.Runtime.CompilerServices;
using Depra.Assets.Idents;
using Depra.Assets.Runtime.Exceptions;
using Depra.Assets.Runtime.Files.Resource.Exceptions;
using JetBrains.Annotations;
using static Depra.Assets.Runtime.Common.Paths;

namespace Depra.Assets.Runtime.Files.Resource
{
	public sealed class ResourcesPath : IAssetIdent
	{
		private static readonly string RESOURCES_FOLDER_PATH = RESOURCES_FOLDER_NAME + Path.AltDirectorySeparatorChar;

		public static ResourcesPath Empty => new(string.Empty);
		public static ResourcesPath Invalid => new(nameof(Invalid));

		public static implicit operator ResourcesPath(string relativePath) => new(relativePath);

		public ResourcesPath(string relativePath) =>
			Initialize(Utility.CombineProjectPath(relativePath));

		public ResourcesPath(string name, string relativeDirectory = null, string extension = null) =>
			Initialize(Utility.CombineProjectPath(relativeDirectory, name, extension));

		public string ProjectPath { get; private set; }
		public string RelativePath { get; private set; }

		[UsedImplicitly]
		public string AbsolutePath { get; private set; }

		public DirectoryInfo Directory { get; private set; }

		string IAssetIdent.Uri => AbsolutePath;
		string IAssetIdent.RelativeUri => RelativePath;

		private void Initialize(string projectPath)
		{
			ProjectPath = projectPath;
			RelativePath = Utility.FindRelativePath(ProjectPath);
			AbsolutePath = Path.GetFullPath(ProjectPath);
			var absoluteDirectoryPath = Path.GetDirectoryName(AbsolutePath);
			Directory = new DirectoryInfo(absoluteDirectoryPath!);
		}

		internal static class Utility
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static string CombineProjectPath(string relativePath) =>
				Path.Combine(ASSETS_FOLDER_NAME, RESOURCES_FOLDER_NAME, relativePath);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static string CombineProjectPath(string directory, string name, string extension = null) =>
				CombineProjectPath(Path.Combine(directory ?? string.Empty, name + extension));

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static string FindRelativePath(string projectPath)
			{
				Guard.AgainstEmptyString(projectPath, () => new NullReferenceException(nameof(projectPath)));

				projectPath = ToUnixPath(projectPath);
				var folderIndex = projectPath.IndexOf(RESOURCES_FOLDER_PATH, StringComparison.OrdinalIgnoreCase);

				Guard.AgainstEqual(folderIndex, -1, () => new PathDoesNotContainResourcesFolder(projectPath));

				folderIndex += RESOURCES_FOLDER_PATH.Length;
				var length = projectPath.Length - folderIndex;
				length -= projectPath.Length - projectPath.LastIndexOf('.');

				return projectPath.Substring(folderIndex, length);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal static string ToUnixPath(string path) => path.Replace('\\', '/');
		}
	}
}