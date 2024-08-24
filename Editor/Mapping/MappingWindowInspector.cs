using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Translations.Editor.Mapping.Inspectors;
using Translations.Mapping;
using UnityEditor;
using UnityEngine;

namespace Translations.Editor.Mapping
{
    internal class MappingWindowInspector
    {
        public MappingWindowInspector(MappingWindow window)
        {
            this.window = window;

            inspectors = new MappingWindowObjectInspector[]
            {
                new MappingGroupInspector(this),
                new MappingItemInspector(this),
                new MappingDynamicValueInspector(this),
            };
        }

        public MappingWindow window;

        private IEnumerable<object> _selectedObjects = Array.Empty<object>();
        public IEnumerable<object> SelectedObjects 
        { 
            get => _selectedObjects; 
            private set
            {
                _currentInspector?.Uninitialize();
                _selectedObjects = value;
                _currentInspector = FindInspectorForObjects(_selectedObjects);
                _currentInspector?.Initialize();
            }
        }

        MappingWindowObjectInspector[] inspectors;

        Vector2 _scroll;
        MappingWindowObjectInspector _currentInspector;

        MappingWindowObjectInspector FindInspectorForObjects(IEnumerable<object> obj) =>
            inspectors
            .Where(x => x.ShouldOpen(obj))
            .FirstOrDefault();

        public void Initialize()
        {
            window.tree.OnSelectionChanged += Tree_OnSelectionChanged;

            SelectedObjects = SelectedObjects;
        }

        private void Tree_OnSelectionChanged(IEnumerable<MappingTree.Item> obj)
        {
            SelectedObjects = obj
                .Select(x => x.Object)
                .Where(x => x !=  null);
        }

        public float Width { get; set; } = 350f;

        public void OnGUI()
        {
            using (var scroll =  new GUILayout.ScrollViewScope(_scroll))
            {
                using (new GUILayout.VerticalScope(new GUIStyle()
                {
                    fixedWidth = Width,
                }))
                {
                    _scroll = scroll.scrollPosition;

                    if (SelectedObjects.Count() > 1)
                    {
                        GUILayout.Label("Multiple items selected");
                        return;
                    }

                    if (_currentInspector != null)
                    {
                        var labelWidth = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = 80f;

                        EditorGUI.BeginChangeCheck();
                        _currentInspector.OnGUI();
                        if (EditorGUI.EndChangeCheck())
                            window.SetAssetDirty();

                        EditorGUIUtility.labelWidth = labelWidth;
                    }

                    GUILayout.FlexibleSpace();
                }
            }
        }
    }
}