using Translations.Mapping.Values;
using UnityEditor;

namespace Translations.Editor.Mapping.Values
{
    internal class MappingValueTextEditor : MappingValueEditor<MappingValueText>
    {
        public override void OnGUI(MappingValueText value)
        {
            EditorGUILayout.LabelField("Value");
            value.value = EditorGUILayout.TextArea(value.value, ToolGuiUtility.TextAreaOptions);
        }
    }
}