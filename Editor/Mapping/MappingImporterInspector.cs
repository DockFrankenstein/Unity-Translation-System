using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEditor;

namespace Translations.Editor.Mapping
{
    [CustomEditor(typeof(MappingImporter))]
    internal class MappingImporterInspector : AssetImporterEditor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Editor", GUILayout.Height(24f)))
                MappingWindow.OpenAsset(assetTarget as Translations.Mapping.Mapping);
            
            ApplyRevertGUI();
        }

        public override bool HasModified() =>
            false;
    }
}