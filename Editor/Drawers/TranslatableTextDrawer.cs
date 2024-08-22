using UnityEditor;
using UnityEngine;

namespace Translations.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(TranslatableText))]
    internal class TranslatableTextDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        TranslatableText target;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var tagProperty = property.FindPropertyRelative("tag");
            bool tagSelected = !string.IsNullOrWhiteSpace(tagProperty.stringValue);
            bool tagExists = TranslationSettings.Instance.mapping.FindItem(tagProperty.stringValue) != null;

            var borderRect = EditorGUI.PrefixLabel(position, new GUIContent("Asd"), EditorStyles.label);

            var colorTagRect = borderRect.ResizeToLeft(8f);
            var colorTagDeviderRect = borderRect.ResizeToRight(0f);

            var tagRect = borderRect
                .BorderLeft(tagSelected ? colorTagRect.width : 0f)
                .BorderLeft(EditorGUIUtility.standardVerticalSpacing);

            ToolGuiUtility.DrawColor(borderRect, ToolGuiUtility.DarkBackgroundColor);

            if (GUI.Button(tagRect, tagSelected ? tagProperty.stringValue : "NONE SELECTED", EditorStyles.label))
            {
                TranslatableTextExplorer.Open(borderRect, tagProperty.stringValue, a =>
                {
                    tagProperty.stringValue = a;
                    property.serializedObject.ApplyModifiedProperties();
                });
            }

            if (tagSelected)
                ToolGuiUtility.DrawColor(colorTagRect, tagExists ?
                    new Color(154f / 255f, 157f / 255f, 255f / 255f) :
                    new Color(255f / 255f, 114f / 255f, 117f / 255f));
            
            ToolGuiUtility.BorderAround(borderRect);
            ToolGuiUtility.VerticalLine(colorTagDeviderRect);
        }

        static class Style
        {
            public static GUIStyle Background => new GUIStyle()
            {
                normal = new GUIStyleState()
                {
                    background = ToolGuiUtility.DarkBackgroundTexture,
                }
            };
        }
    }
}