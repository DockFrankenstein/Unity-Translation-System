using System;
using System.Collections.Generic;
using System.Linq;
using Translations.Mapping;
using UnityEditor;
using Translations.Utility;
using Translations.Mapping.Values;
using UnityEngine;
using UnityEditor.UIElements;
using Translations.Editor.Mapping.Values;

namespace Translations.Editor.Mapping.Inspectors
{
    internal class MappingItemInspector : MappingWindowObjectInspector
    {
        public MappingItemInspector(MappingWindowInspector inspector) : base(inspector) 
        {
            _mappingValueTypes = TypeFinder
                .FindAllTypes<MappingValue>()
                .Where(x => !x.IsAbstract)
                .ToArray();

            _mappingValueTypeNames = _mappingValueTypes
                .Select(x => (MappingValue)TypeFinder.CreateConstructorFromType(x))
                .Select(x => x?.Name ?? string.Empty)
                .ToArray();
        }

        public override bool ShouldOpen(IEnumerable<object> selected) =>
            selected.Count() == 1 &&
            selected.First() is MappingItem;

        MappingItem item;

        Type[] _mappingValueTypes;
        string[] _mappingValueTypeNames;
        GUIContent[] _valueTypeContent;
        int valueTypeIndex;

        MappingValueEditor _valueEditor;

        public override void Initialize()
        {
            item = Inspector.SelectedObjects.First() as MappingItem;

            UpdateValueTypeContent();

            valueTypeIndex = _mappingValueTypes.ToList().IndexOf(item.defaultValue?.GetType() ?? null);
            if (valueTypeIndex == -1)
                valueTypeIndex = 0;

            UpdateValueEditor();
        }

        void UpdateValueTypeContent()
        {
            var noneSelected = !_mappingValueTypes.Contains(item.defaultValue?.GetType());

            List<GUIContent> content = new List<GUIContent>();

            if (noneSelected)
                content.Add(new GUIContent("None"));

            content.AddRange(_mappingValueTypeNames.Select(x => new GUIContent(x)));

            _valueTypeContent = content.ToArray();
        }

        void UpdateValueEditor() =>
            _valueEditor = MappingValueEditor.GetEditor(item?.defaultValue);

        public override void Uninitialize()
        {
            item = null;
        }

        string test;

        public override void OnGUI()
        {
            item.tag = EditorGUILayout.DelayedTextField("Tag", item.tag);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Default Value", EditorStyles.boldLabel);
            var newValueTypeIndex = EditorGUILayout.Popup(new GUIContent("Type"), valueTypeIndex, _valueTypeContent);

            if (newValueTypeIndex != valueTypeIndex)
            {
                if (!_mappingValueTypes.Contains(item.defaultValue?.GetType()))
                    newValueTypeIndex--;

                valueTypeIndex = newValueTypeIndex;
                item.defaultValue = (MappingValue)TypeFinder.CreateConstructorFromType(_mappingValueTypes[valueTypeIndex]);
                UpdateValueTypeContent();
                UpdateValueEditor();
            }

            if (_valueEditor != null)
                _valueEditor.OnGUIValue(item.defaultValue);
        }
    }
}
