using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace Translations.Editor
{
    public class TranslationSettingsProvider : SettingsProvider
    {
        public TranslationSettingsProvider(string path, SettingsScope scopes) : base(path, scopes) { }

        TranslationSettings settings;
        SerializedObject serializedSettings;

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);
            settings = TranslationSettings.Instance;
            serializedSettings = new SerializedObject(settings);
        }

        public override void OnGUI(string searchContext)
        {
            ToolGuiUtility.DrawObjectsProperties(serializedSettings);

            if (serializedSettings.hasModifiedProperties)
                serializedSettings.ApplyModifiedProperties();
        }

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            SettingsProvider provider = new TranslationSettingsProvider("Project/Translation System", SettingsScope.Project)
            {
                label = "Translation System",
                keywords = new HashSet<string>(new[] { "Translation", "DF" })
            };

            return provider;
        }
    }
}
