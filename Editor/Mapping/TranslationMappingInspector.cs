using UnityEditor;
using Translations.Mapping;

namespace Translations.Editor.Mapping
{
    [CustomEditor(typeof(TranslationMapping))]
    internal class TranslationMappingInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI() { }
    }
}
