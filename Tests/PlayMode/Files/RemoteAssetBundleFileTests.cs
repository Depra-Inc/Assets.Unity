// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Depra.Assets.Extensions;
using Depra.Assets.Files.Bundles.Files;
using Depra.Assets.Files.Bundles.Idents;
using Depra.Assets.Files.Bundles.Sources;
using Depra.Assets.Tests.PlayMode.Stubs;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Depra.Assets.Tests.PlayMode.Files
{
	[TestFixture(TestOf = typeof(AssetBundleFromWeb))]
	internal sealed class RemoteAssetBundleFileTests
	{
		private const string TEST_BUNDLE_NAME = "test";

		private Stopwatch _stopwatch;
		private Process _serverProcess;
		private TestAssetBundlesDirectory _assetBundlesDirectory;

		[OneTimeSetUp]
		public void OneTimeSetup() => _stopwatch = new Stopwatch();

		[SetUp]
		public void Setup()
		{
			_assetBundlesDirectory = new TestAssetBundlesDirectory();
			_serverProcess = Process.Start(new ProcessStartInfo
			{
				FileName = "cmd.exe",
				Arguments = $"/c http-server \"{_assetBundlesDirectory.AbsolutePath}\"",
				UseShellExecute = false,
				RedirectStandardOutput = true
			});

			WaitForServerToBeReady();
		}

		[TearDown]
		public void Teardown()
		{
			if (_serverProcess is not { HasExited: false })
			{
				return;
			}

			_serverProcess.CloseMainWindow();
			_serverProcess.WaitForExit();
		}

		[UnityTest]
		public IEnumerator LoadAsync_ShouldSucceed() => ATask.ToCoroutine(async () =>
		{
			// Arrange.
			var bundleIdent = new AssetBundleIdent(TEST_BUNDLE_NAME, _assetBundlesDirectory.ProjectRelativePath);
			var bundleFile = new AssetBundleFile(bundleIdent, new AssetBundleFromWeb());

			// Act.
			_stopwatch.Restart();
			var loadedBundle = await bundleFile.LoadAsync();
			_stopwatch.Stop();

			// Assert.
			Assert.That(bundleFile.IsLoaded);
			Assert.That(loadedBundle, Is.Not.Null);
			Assert.IsInstanceOf<AssetBundle>(loadedBundle);

			// Debug.
			TestContext.WriteLine($"Loaded bundle {loadedBundle.name} " +
			                      $"by path: {bundleFile.Ident.Uri} " +
			                      $"in {_stopwatch.ElapsedMilliseconds} ms.");

			await Task.Yield();
		});

		private void WaitForServerToBeReady() => Thread.Sleep(2000);
	}
}