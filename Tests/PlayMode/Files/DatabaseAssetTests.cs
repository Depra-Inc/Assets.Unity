// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Depra.Assets.Exceptions;
using Depra.Assets.Files.Database;
using NUnit.Framework;
using UnityEngine;

namespace Depra.Assets.Tests.PlayMode.Files
{
	[TestFixture(TestOf = typeof(DatabaseAsset<>))]
	internal sealed class DatabaseAssetTests
	{
		[Test]
		public void LoadAsync_ShouldThrowsAssetCanNotBeLoadedException()
		{
			// Arrange:
			var invalidIdent = DatabaseAssetIdent.Empty;
			var databaseAsset = new DatabaseAsset<NonExistentScriptableAsset>(invalidIdent);

			// Act:
			[SuppressMessage("ReSharper", "MoveLocalFunctionAfterJumpStatement")]
			async Task Act() => await databaseAsset.LoadAsync();

			// Assert:
			Assert.ThrowsAsync<AssetCanNotBeLoaded>(Act);
		}

		private sealed class NonExistentScriptableAsset : ScriptableObject { }
	}
}