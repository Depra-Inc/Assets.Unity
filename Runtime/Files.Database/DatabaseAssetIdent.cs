// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.IO;
using Depra.Assets.Idents;
using JetBrains.Annotations;

namespace Depra.Assets.Unity.Runtime.Files.Database
{
	public readonly struct DatabaseAssetIdent : IAssetIdent
	{
		public static DatabaseAssetIdent Empty = new();

		public static implicit operator DatabaseAssetIdent(string relativePath) => new(relativePath);

		public DatabaseAssetIdent(string relativePath) : this(Path.GetDirectoryName(relativePath),
			Path.GetFileNameWithoutExtension(relativePath),
			Path.GetExtension(relativePath)) { }

		public DatabaseAssetIdent(string relativeDirectory, string name, string extension)
		{
			Name = name;
			Extension = extension;
			NameWithExtension = Name + Extension;

			RelativePath = Path.Combine(relativeDirectory, NameWithExtension).Replace(@"\", "/");

			AbsolutePath = Path.GetFullPath(RelativePath).Replace(@"\", "/");
			AbsoluteDirectoryPath = Path.GetFullPath(relativeDirectory).Replace(@"\", "/");
			Directory = new DirectoryInfo(AbsoluteDirectoryPath);
		}

		public string Name { get; }

		[UsedImplicitly]
		public string Extension { get; }

		[UsedImplicitly]
		public string NameWithExtension { get; }

		public string RelativePath { get; }
		public string AbsolutePath { get; }

		[UsedImplicitly]
		public string AbsoluteDirectoryPath { get; }

		internal DirectoryInfo Directory { get; }

		string IAssetIdent.Uri => AbsolutePath;
		string IAssetIdent.RelativeUri => RelativePath;

		public bool Exists() => File.Exists(AbsolutePath);
	}
}