using System;

namespace Depra.Assets.Runtime.Exceptions
{
	internal sealed class AssetCanNotBeLoaded : Exception
	{
		public AssetCanNotBeLoaded(string message) : base(message) { }
	}
}