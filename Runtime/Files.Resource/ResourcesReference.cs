﻿// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Depra.Assets.Files.Resource
{
	[Serializable]
	public sealed class ResourcesReference<TAsset> : ResourcesReference where TAsset : Object { }

	[Serializable]
	public class ResourcesReference : ISerializationCallbackReceiver
	{
#if UNITY_EDITOR
		[SerializeField] internal Object _objectAsset;
#endif
		[SerializeField] internal string _projectPath;

		internal protected ResourcesReference() { }

		public ResourcesPath Path => new(GetProjectPath());
		public bool IsNull => string.IsNullOrEmpty(_projectPath);

		private string GetProjectPath()
		{
#if UNITY_EDITOR
			return AssetDatabase.GetAssetPath(_objectAsset);
#else
            return _projectPath;
#endif
		}

#if UNITY_EDITOR
		private void OnAfterDeserializeHandler()
		{
			UpdateProjectPath();
			EditorApplication.update -= OnAfterDeserializeHandler;
		}

		private void UpdateProjectPath()
		{
			if (_objectAsset == null)
			{
				return;
			}

			var projectPath = AssetDatabase.GetAssetPath(_objectAsset);
			if (projectPath.Equals(_projectPath))
			{
				return;
			}

			_projectPath = projectPath;
			if (Application.isPlaying == false)
			{
				EditorSceneManager.MarkAllScenesDirty();
			}
		}
#endif

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
#if UNITY_EDITOR
			UpdateProjectPath();
#endif
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
#if UNITY_EDITOR
			// OnAfterDeserialize is called in the deserialization thread so we can't touch Unity API.
			// Wait for the next update frame to do it.
			EditorApplication.update += OnAfterDeserializeHandler;
#endif
		}
	}
}