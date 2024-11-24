using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Translations.Mapping;
using UnityEditor;

namespace Translations.Editor.Mapping.Inspectors
{
    internal class MappingDynamicValueInspector : MappingWindowObjectInspector
    {
        public MappingDynamicValueInspector(MappingWindowInspector inspector) : base(inspector)
        {
        }

        public override bool ShouldOpen(IEnumerable<object> selected) =>
            selected.Count() == 1 &&
            selected.First() is MappingDynamicValue;

        MappingDynamicValue value;

        public override void Initialize()
        {
            value = Inspector.SelectedObjects.First() as MappingDynamicValue;
        }

        public override void Uninitialize()
        {
            value = null;
        }

        public override void OnGUI()
        {
            value.tag = EditorGUILayout.DelayedTextField("Tag", value.tag);

            EditorGUILayout.LabelField("Note");
            value.note = EditorGUILayout.TextArea(value.note, ToolGuiUtility.TextAreaOptions);
        }
    }
}
