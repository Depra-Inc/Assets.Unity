// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.IO;
using Depra.Assets.Idents;
using Depra.Assets.Unity.Runtime.Exceptions;
using Depra.Assets.Unity.Runtime.Files.Resource.Exceptions;
using static Depra.Assets.Unity.Runtime.Common.Paths;

namespace Depra.Assets.Unity.Runtime.Files.Resource
{
	public sealed class ResourcesPath : ProjectPathInfo, IAssetIdent
	{
		private static readonly string RESOURCES_FOLDER_PATH = RESOURCES_FOLDER_NAME + Path.DirectorySeparatorChar;

		public static ResourcesPath Empty => new(string.Empty);
		public static ResourcesPath Invalid => new(nameof(Invalid));

		public static implicit operator ResourcesPath(string relativePath) => new(relativePath);

		public ResourcesPath(string relativePath) : base(CombineProjectPath(relativePath)) =>
			RelativePath = Utility.FindRelativePath(ProjectPath);

		public ResourcesPath(string name, string relativeDirectory = null, string extension = null) :
			base(CombineProjectPath(relativeDirectory, name, extension)) =>
			RelativePath = Utility.FindRelativePath(ProjectPath);

		public string RelativePath { get; }

		string IAssetIdent.Uri => AbsolutePath;

		string IAssetIdent.RelativeUri => RelativePath;

		internal static class Utility
		{
			public static string FindRelativePath(string projectPath)
			{
				Guard.AgainstEmptyString(projectPath, () => new NullReferenceException(nameof(projectPath)));
				var folderIndex = projectPath.IndexOf(RESOURCES_FOLDER_PATH, StringComparison.Ordinal);
				Guard.AgainstEqual(folderIndex, -1, () => new PathDoesNotContainResourcesFolder(projectPath));

				folderIndex += RESOURCES_FOLDER_PATH.Length;
				var length = projectPath.Length - folderIndex;
				length -= projectPath.Length - projectPath.LastIndexOf('.');

				return projectPath.Substring(folderIndex, length);
			}
		}
	}

	public class ProjectPathInfo : IAssetIdent
	{
		protected static string CombineProjectPath(string relativePath) =>
			Path.Combine(ASSETS_FOLDER_NAME, RESOURCES_FOLDER_NAME, relativePath);

		protected static string CombineProjectPath(string directory, string name, string extension = null) =>
			CombineProjectPath(Path.Combine(directory ?? string.Empty, name + extension));

		protected ProjectPathInfo(string relativePath)
		{
			ProjectPath = relativePath;
			AbsolutePath = Path.GetFullPath(ProjectPath);
			var absoluteDirectoryPath = Path.GetDirectoryName(AbsolutePath);
			Directory = new DirectoryInfo(absoluteDirectoryPath!);
		}

		public string ProjectPath { get; }

		public DirectoryInfo Directory { get; }

		protected string AbsolutePath { get; }

		string IAssetIdent.Uri => AbsolutePath;

		string IAssetIdent.RelativeUri => ProjectPath;
	}
}