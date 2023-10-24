// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System.Threading.Tasks;
using Depra.Assets.Exceptions;
using Depra.Assets.Files.Database;
using NUnit.Framework;
using UnityEngine;

namespace Depra.Assets.Tests.PlayMode.Files
{
	internal sealed class DatabaseAssetTests
	{
		[Test]
		public void LoadAsync_ShouldThrowsAssetCanNotBeLoadedException()
		{
			// Arrange:
			var invalidIdent = DatabaseAssetUri.Empty;
			var databaseAsset = new DatabaseAsset<NonExistentScriptableAsset>(invalidIdent);

			// Act:
#pragma warning disable CS0618 // Type or member is obsolete
			async Task Act() => await databaseAsset.LoadAsync();
#pragma warning restore CS0618 // Type or member is obsolete

			// Assert:
			Assert.ThrowsAsync<AssetCanNotBeLoaded>(Act);
		}

		private sealed class NonExistentScriptableAsset : ScriptableObject { }
	}
}