using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEditor;
using Translations.Mapping;

namespace Translations.Editor.Mapping
{
    [CustomEditor(typeof(TranslationMappingImporter))]
    internal class TranslationMappingImporterInspector : AssetImporterEditor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Editor", GUILayout.Height(24f)))
                TranslationMappingWindow.OpenAsset(assetTarget as TranslationMapping);
            
            ApplyRevertGUI();
        }
    }
}