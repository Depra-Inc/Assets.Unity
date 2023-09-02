using System.Threading;
using System.Threading.Tasks;
using Depra.Assets.Delegates;
using UnityEngine;

namespace Depra.Assets.Runtime.Files.Resource
{
	internal static class ResourceRequestExtensions
	{
		public async static Task<Object> ToTask(this ResourceRequest self, DownloadProgressDelegate progress = null,
			CancellationToken cancellationToken = default)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return await Task.FromCanceled<Object>(cancellationToken);
			}

			if (self.isDone)
			{
				return await Task.FromResult(self.asset);
			}

			return await ResourceRequestConfiguredSource.Create(self, progress, cancellationToken, out _);
		}
	}
}