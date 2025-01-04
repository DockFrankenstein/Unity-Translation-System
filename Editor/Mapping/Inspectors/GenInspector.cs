using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Asset = Translations.Mapping.Mapping;

namespace Translations.Editor.Mapping.Inspectors
{
    internal class GenInspector : MappingWindowObjectInspector
    {
        public GenInspector(MappingWindowInspector inspector) : base(inspector)
        {
        }

        Asset _mapping;

        public override void Initialize()
        {
            _mapping = Inspector.window.asset;
            base.Initialize();
        }

        public override void Uninitialize()
        {
            _mapping = null;
            base.Uninitialize();
        }

        public override void OnGUI()
        {
            _mapping.genBehaviour = (Asset.DefaultGenerationBehaviour)EditorGUILayout.EnumPopup("Default Behaviour", _mapping.genBehaviour);
            
            if (_mapping.genBehaviour == Asset.DefaultGenerationBehaviour.SingleFile)
                _mapping.genSingleFileName = EditorGUILayout.DelayedTextField("Single File Name", _mapping.genSingleFileName);
        
            _mapping.genLowercase = EditorGUILayout.Toggle("Lowercase Files", _mapping.genLowercase);
        }

        public override bool ShouldOpen(IEnumerable<object> selected) =>
            selected.FirstOrDefault() is string s &&
            s == "gen";
    }
}