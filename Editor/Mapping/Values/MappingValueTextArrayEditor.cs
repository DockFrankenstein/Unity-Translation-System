using Translations.Mapping.Values;
using UnityEditorInternal;
using System.Collections.Generic;
using UnityEditor;

namespace Translations.Editor.Mapping.Values
{
    internal class MappingValueTextArrayEditor : MappingValueEditor<MappingValueTextArray>
    {
        ReorderableList list;
        List<string> items;

        public override void Initialize(MappingValueTextArray value)
        {
            items = new List<string>(value.array);
            list = new ReorderableList(items, typeof(string));

            list.drawHeaderCallback += (rect) =>
                EditorGUI.LabelField(rect, "Values");

            list.drawElementCallback += (rect, index, active, focused) =>
            {
                using (var change = new EditorGUI.ChangeCheckScope())
                {
                    items[index] = EditorGUI.DelayedTextField(rect, items[index]);
                    if (change.changed)
                        list.onChangedCallback?.Invoke(list);
                }
            };

            list.onChangedCallback += _ =>
            {
                value.array = items.ToArray();
            };
        }

        public override void OnGUI(MappingValueTextArray value)
        {
            list.DoLayoutList();
        }
    }
}