using UnityEditor;
using Translations.Mapping;

namespace Translations.Editor.Mapping
{
    [CustomEditor(typeof(Translations.Mapping.Mapping))]
    internal class MappingInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI() { }
    }
}
