using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Generating", EditorStyles.boldLabel);

            DrawSerializerPopup();
            group.automaticName = EditorGUILayout.Toggle("Auto File Name", group.automaticName);

            DrawFileName();
        }

        void DrawSerializerPopup()
        {
            var serialization = TranslationSettings.Instance.serialization;
            var serializers = serialization.serializers.Select(x => x.Id).ToList();
            
            if (serializers.Count == 0)
            {
                using (new EditorGUI.DisabledGroupScope(true))
                    EditorGUILayout.Popup("Serializer", 0, new string[] { "None Avaliable" });

                return;
            }
            
            var index = serializers.IndexOf(group.serializerType);
            if (index == -1) index = 0;
            var newIndex = EditorGUILayout.Popup("Serializer", index, serializers.ToArray());

            if (index == newIndex) return;
            group.serializerType = serializers[newIndex];

            //Update file extension
            if (index >= 0 && index < serializers.Count &&
                newIndex >= 0 && newIndex < serializers.Count)
            {
                if (!serialization.serializers[index].FileExtensions
                    .Any(x => group.fileName.EndsWith($".{x}"))) return;

                if (serialization.serializers[index].FileExtensions.Length == 0)
                    return;
                
                group.fileName = Path.ChangeExtension(group.fileName, serialization.serializers[index].FileExtensions[0]);
            }          
        }

        void DrawFileName()
        {
            if (group.automaticName)
            {
                using (new EditorGUI.DisabledGroupScope(true))
                {
                    EditorGUILayout.DelayedTextField("File Name", "Automatic");
                }

                return;
            }

            group.fileName = EditorGUILayout.DelayedTextField("File Name", group.fileName);
        }
    }
}
