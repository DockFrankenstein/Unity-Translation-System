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

            var borderRect = EditorGUI.PrefixLabel(position, new GUIContent("Asd"), EditorStyles.label);

            var colorTagRect = borderRect.ResizeToLeft(8f);
            var colorTagDeviderRect = borderRect.ResizeToRight(0f);

            var tagRect = borderRect
                .BorderLeft(tagSelected ? colorTagRect.width : 0f)
                .BorderLeft(EditorGUIUtility.standardVerticalSpacing);

            ToolGuiUtility.DrawColor(borderRect, ToolGuiUtility.DarkBackgroundColor);

            if (GUI.Button(tagRect, tagSelected ? tagProperty.stringValue : "NONE SELECTED", EditorStyles.label))
            {
                TranslatableTextExplorer.Open(borderRect, a =>
                {
                    tagProperty.stringValue = a;
                    property.serializedObject.ApplyModifiedProperties();
                });
            }

            if (tagSelected)
                ToolGuiUtility.DrawColor(colorTagRect, new Color(30f / 255f, 89f / 255f, 250f / 255f));
            
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