using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Depra.Assets.Unity.Editor.Ident
{
    internal static class SerializedPropertyExtensions
    {
        private static readonly Regex ARRAY_PATH_REGEX =
            new(@"(?<arrayName>\w+)\[(?<index>\d+)\]", RegexOptions.Compiled);

        private static readonly Regex AUTO_PROPERTY_NAME_REGEX = new("<(.+?)>k__BackingField");

        internal static object GetValue(this SerializedProperty property,
            Func<IEnumerable<string>, IEnumerable<string>> modifier = null)
        {
            IEnumerable<string> path = property.propertyPath.Replace(".Array.data[", "[").Split('.');
            if (modifier != null)
            {
                path = modifier(path);
            }

            var target = (object) property.serializedObject.targetObject;
            return GetValueRecursive(target, path);
        }

        private static object GetValueRecursive(object target, IEnumerable<string> path)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var pathList = path.ToList();
            var head = pathList.FirstOrDefault();
            if (head == null)
            {
                return target;
            }

            var arrayMatch = ARRAY_PATH_REGEX.Match(head);
            if (arrayMatch.Success)
            {
                var autoPropertyMatch = AUTO_PROPERTY_NAME_REGEX.Match(head);
                head = autoPropertyMatch.Success
                    ? autoPropertyMatch.Groups[1].Value
                    : arrayMatch.Groups["arrayName"].Value;

                var field = target.GetType().FieldRecursive(head);
                var property = target.GetType().Property(head);
                var array = (field != null ? field.GetValue(target) : property.GetValue(target)) as IEnumerable;
                var index = int.Parse(arrayMatch.Groups["index"].Value);

                target = array.ElementAtOrDefault(index);
            }
            else
            {
                target = target.GetType().FieldRecursive(head).GetValue(target);
            }

            return GetValueRecursive(target, pathList.Skip(1));
        }

        private static object ElementAtOrDefault(this IEnumerable sequence, int index)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException(nameof(sequence));
            }

            foreach (var element in sequence)
            {
                if (index == 0)
                {
                    return element;
                }

                index--;
            }

            return null;
        }

        private static FieldInfo FieldRecursive(this Type type, string fieldName)
        {
            var field = type.Field(fieldName);
            if (field != null)
            {
                return field;
            }

            return type.BaseType == typeof(object) ? null : type.BaseType.FieldRecursive(fieldName);
        }

        private static FieldInfo Field(this Type type, string fieldName)
            => type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        private static PropertyInfo Property(this Type type, string propertyName)
            => type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
    }
}