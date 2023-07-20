using Depra.Assets.Unity.Runtime.Files.Resource;
using UnityEditor;
using UnityEngine;

namespace Depra.Assets.Unity.Editor.Ident
{
    [CustomPropertyDrawer(typeof(ResourcesReference), true)]
    internal sealed class ResourcesReferenceDrawer : PropertyDrawer
    {
        private SerializedProperty _objectAssetProperty;
        private SerializedProperty _projectPathProperty;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _objectAssetProperty = property.FindPropertyRelative("_objectAsset");
            _projectPathProperty = property.FindPropertyRelative("_projectPath");

            position.height = EditorGUIUtility.singleLineHeight;
            DrawObjectReferenceField(position, property, label);
        }

        private void DrawObjectReferenceField(Rect position, SerializedProperty property, GUIContent label)
        {
            var objectType = property.GetValue().GetType().GenericTypeArguments[0];
            var propertyValue = _objectAssetProperty.objectReferenceValue;

            EditorGUI.BeginChangeCheck();
            propertyValue = EditorGUI.ObjectField(position, label, propertyValue, objectType, false);

            if (EditorGUI.EndChangeCheck())
            {
                ApplyPropertyChange(propertyValue);
            }
        }

        private void ApplyPropertyChange(Object objectAsset)
        {
            var assetProjectPath = string.Empty;

            if (objectAsset != null)
            {
                var projectPath = AssetDatabase.GetAssetPath(objectAsset);
                if (Resources.Load(new ResourcesPath(projectPath).FindRelativePath()) != null)
                {
                    assetProjectPath = projectPath;
                }
            }

            _projectPathProperty.stringValue = assetProjectPath;
            _objectAssetProperty.objectReferenceValue = objectAsset;
        }
    }
}