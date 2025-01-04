using System;
using System.Collections.Generic;
using System.Linq;
using Translations.Serialization;
using Translations.Serialization.Serializers;
using Translations.Utility;
using UnityEditor;
using UnityEngine;

namespace Translations.Editor.Serialization
{
    [CustomEditor(typeof(TranslationSerialization))]
    public class TranslationSerializationInspector : UnityEditor.Editor
    {
        Type[] serializerTypes;

        public override void OnInspectorGUI()
        {
            var asset = (TranslationSerialization)target;

            if (serializerTypes == null)
                serializerTypes = TypeFinder.FindAllTypes<TranslationSerializer>()
                    .Where(x => !x.IsAbstract)
                    .ToArray();

            ToolGuiUtility.DrawPropertiesFromStart(serializedObject, nameof(TranslationSerialization.infoAuthorsField));

            EditorGUILayout.Space();
            EditorGUILayout.PrefixLabel("Serializers");

            var serializers = new List<TranslationSerializer>(asset.serializers);
            foreach (var item in serializers)
            {
                GUILayout.Label(string.Empty, GUILayout.Height(28f));
                var rect = GUILayoutUtility.GetLastRect();
                var buttonRect = rect.ResizeToRight(28f);
                var buttonSeparatorRect = buttonRect.ResizeToLeft(0f);
                ToolGuiUtility.DrawColor(rect, ToolGuiUtility.DarkBackgroundColor);

                GUI.Label(rect, item.name);

                if (GUI.Button(buttonRect, ToolGuiUtility.MinusIcon, EditorStyles.centeredGreyMiniLabel))
                {
                    asset.serializers.Remove(item);
                    DestroyImmediate(item, true);
                    AssetDatabase.SaveAssets();
                }

                ToolGuiUtility.BorderAround(rect);
                ToolGuiUtility.VerticalLine(buttonSeparatorRect);
            }

            if (GUILayout.Button("Add", GUILayout.Height(36f)))
            {
                var menu = new GenericMenu();

                var addedTypes = asset.serializers.Select(x => x.GetType());

                foreach (var type in serializerTypes)
                    menu.AddToggableItem(type.Name, false, () => AddSerializer(type), !addedTypes.Contains(type));

                menu.ShowAsContext();
            }
        }

        void AddSerializer(Type type)
        {
            var so = (TranslationSerializer)ScriptableObject.CreateInstance(type);
            so.name = type.Name;
            AssetDatabase.AddObjectToAsset(so, target);
            (target as TranslationSerialization).serializers.Add(so);
            AssetDatabase.SaveAssets();

            EditorUtility.SetDirty(target);
            EditorUtility.SetDirty(so);
        }
    }
}