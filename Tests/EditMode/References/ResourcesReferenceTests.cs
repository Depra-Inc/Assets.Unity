using Depra.Assets.Runtime.Files.Resource;
using NUnit.Framework;

namespace Tests.EditMode
{
	[TestFixture(TestOf = typeof(ResourcesReference))]
	internal sealed class ResourcesReferenceTests
	{
		[Test]
		public void IsNull_TrueWhenProjectPathIsEmpty()
		{
			// Arrange.
			var reference = new ResourcesReference { _projectPath = string.Empty };

			// Act.
			var isNull = reference.IsNull;

			// Assert.
			Assert.IsTrue(isNull);
		}

		[Test]
		public void IsNull_FalseWhenProjectPathIsNotEmpty()
		{
			// Arrange.
			var resourcePath = "Assets/SomeFolder/SomeAsset.prefab";
			var reference = new ResourcesReference { _projectPath = resourcePath };

			// Act.
			var isNull = reference.IsNull;

			// Assert.
			Assert.IsFalse(isNull);
		}
	}
}