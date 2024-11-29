using Translations.Mapping.Values;
using UnityEditor;

namespace Translations.Editor.Mapping.Values
{
    internal class MappingValueTextEditor : MappingValueEditor<MappingValueText>
    {
        public override void OnGUI(MappingValueText value)
        {
            EditorGUILayout.LabelField("Value");
            value.content = EditorGUILayout.TextArea(value.content, ToolGuiUtility.TextAreaOptions);
        }
    }
}
