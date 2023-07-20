using System;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Depra.Assets.Unity.Runtime.Files.Resource
{
    [Serializable]
    public sealed class ResourcesReference<TAsset> : ResourcesReference where TAsset : Object { }

    [Serializable]
    public class ResourcesReference : ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        [SerializeField] private Object _objectAsset;
#endif
        [SerializeField] private string _projectPath;

        protected ResourcesReference() { }

        public bool IsNull => string.IsNullOrEmpty(_projectPath);

        public string ResourcesPath => new ResourcesPath(ProjectPath).FindRelativePath();

        public string ProjectPath
        {
            get
            {
#if UNITY_EDITOR
                return AssetDatabase.GetAssetPath(_objectAsset);
#else
                return _projectPath;
#endif
            }
        }

        [Obsolete("Needed for the editor, don't use it in runtime code!", true)]
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            UpdateProjectPath();
#endif
        }

        [Obsolete("Needed for the editor, don't use it in runtime code!", true)]
        public void OnAfterDeserialize()
        {
#if UNITY_EDITOR
            // OnAfterDeserialize is called in the deserialization thread so we can't touch Unity API.
            // Wait for the next update frame to do it.
            EditorApplication.update += OnAfterDeserializeHandler;
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
    }
}