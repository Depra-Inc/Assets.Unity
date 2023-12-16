// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Assets.Editor.Extensions;
using Depra.Assets.Files.Resource;
using UnityEditor;
using UnityEngine;

namespace Depra.Assets.Editor.Drawers
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
			var propertyValue = property.GetValue();
			var propertyType = propertyValue.GetType();
			var objectType = propertyType.IsGenericType ? propertyType.GenericTypeArguments[0] : typeof(Object);

			var @object = _objectAssetProperty.objectReferenceValue;

			EditorGUI.BeginChangeCheck();
			@object = EditorGUI.ObjectField(position, label, @object, objectType, false);

			if (EditorGUI.EndChangeCheck())
			{
				ApplyPropertyChange(@object);
			}
		}

		private void ApplyPropertyChange(Object objectAsset)
		{
			var assetProjectPath = string.Empty;

			if (objectAsset != null)
			{
				var projectPath = AssetDatabase.GetAssetPath(objectAsset);
				if (Resources.Load(ResourcesPath.FindRelativePath(projectPath)) != null)
				{
					assetProjectPath = projectPath;
				}
			}

			_projectPathProperty.stringValue = assetProjectPath;
			_objectAssetProperty.objectReferenceValue = objectAsset;
		}
	}
}