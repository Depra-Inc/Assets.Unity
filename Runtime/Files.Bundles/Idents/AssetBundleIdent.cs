// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System.IO;
using Depra.Assets.Idents;
using Depra.Assets.Runtime.Extensions;
using JetBrains.Annotations;

namespace Depra.Assets.Runtime.Files.Bundles.Idents
{
	public sealed record AssetBundleIdent : IAssetIdent
	{
		private const string EXTENSION = ".assetbundle";

		public static AssetBundleIdent Empty => new(string.Empty);
		public static AssetBundleIdent Invalid => new(nameof(Invalid));

		public static implicit operator AssetBundleIdent(string path) => new(path);

		private readonly FileInfo _fileInfo;

		public AssetBundleIdent(string path)
		{
			_fileInfo = new FileInfo(path);
			_fileInfo.Directory.CreateIfNotExists();

			Name = string.IsNullOrEmpty(Extension)
				? _fileInfo.Name
				: _fileInfo.Name.Replace(Extension, string.Empty);

			AbsolutePathWithoutExtension = AbsolutePath.Replace(EXTENSION, string.Empty);
		}

		public AssetBundleIdent(string name, string directory = null) : this(name, directory, EXTENSION) { }

		public AssetBundleIdent(string name, string directory, string extension = null) : this(
			Path.Combine(directory, name + extension)) { }

		[UsedImplicitly]
		public string Name { get; }

		[UsedImplicitly]
		public string Extension => EXTENSION;

		[UsedImplicitly]
		public string NameWithExtension => Name + Extension;

		[UsedImplicitly]
		public string AbsolutePath => _fileInfo.FullName;

		[UsedImplicitly]
		public string AbsoluteDirectoryPath => _fileInfo.DirectoryName;

		public string AbsolutePathWithoutExtension { get; }

		string IAssetIdent.Uri => AbsolutePath;

		string IAssetIdent.RelativeUri => Name;
	}
}