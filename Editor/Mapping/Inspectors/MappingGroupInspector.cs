using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Translations.Mapping;
using UnityEditor;

namespace Translations.Editor.Mapping.Inspectors
{
    internal class MappingGroupInspector : MappingWindowObjectInspector
    {
        public MappingGroupInspector(MappingWindowInspector inspector) : base(inspector)
        {
        }

        public override bool ShouldOpen(IEnumerable<object> selected) =>
            selected.Count() == 1 &&
            selected.First() is MappingGroup;

        MappingGroup group;

        public override void Initialize()
        {
            group = Inspector.SelectedObjects.First() as MappingGroup;
        }

        public override void Uninitialize()
        {
            group = null;
        }

        public override void OnGUI()
        {
            group.name = EditorGUILayout.DelayedTextField("Name", group.name);
        }
    }
}
