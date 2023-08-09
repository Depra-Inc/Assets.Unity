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

		public ResourcesPath(string relativePath) : base(CombineProjectPath(relativePath)) =>
			RelativePath = FindRelativePath();

		public ResourcesPath(string name, string relativeDirectory = null, string extension = null) :
			base(CombineProjectPath(relativeDirectory, name, extension)) =>
			RelativePath = FindRelativePath();

		public string RelativePath { get; }

		string IAssetIdent.Uri => AbsolutePath;

		string IAssetIdent.RelativeUri => RelativePath;

		internal string FindRelativePath()
		{
			Guard.AgainstEmptyString(ProjectPath, () => new NullReferenceException(nameof(ProjectPath)));
			var folderIndex = ProjectPath.IndexOf(RESOURCES_FOLDER_PATH, StringComparison.Ordinal);
			Guard.AgainstEqual(folderIndex, -1, () => new PathDoesNotContainResourcesFolder(ProjectPath));

			folderIndex += RESOURCES_FOLDER_PATH.Length;
			var length = ProjectPath.Length - folderIndex;
			length -= ProjectPath.Length - ProjectPath.LastIndexOf('.');

			return ProjectPath.Substring(folderIndex, length);
		}
	}

	public class ProjectPathInfo
	{
		protected static string CombineProjectPath(string relativePath) =>
			Path.Combine(ASSETS_FOLDER_NAME, RESOURCES_FOLDER_NAME, relativePath);

		protected static string CombineProjectPath(string directory, string name, string extension = null) =>
			CombineProjectPath(Path.Combine(directory ?? string.Empty, name + extension));

		protected ProjectPathInfo(string projectPath)
		{
			ProjectPath = projectPath;
			AbsolutePath = Path.GetFullPath(ProjectPath);
			var absoluteDirectoryPath = Path.GetDirectoryName(AbsolutePath);
			Directory = new DirectoryInfo(absoluteDirectoryPath!);
		}

		public string ProjectPath { get; }

		public DirectoryInfo Directory { get; }

		protected string AbsolutePath { get; }
	}
}