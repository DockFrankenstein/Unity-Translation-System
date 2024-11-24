using System.IO;
using Translations.Mapping;
using Translations.Serialization;
using UnityEngine;

namespace Translations
{
    public class TranslationSettings : ScriptableObject
    {
        public const string DEFAULT_PROJECT_LOCATION = "Resources/Project Settings/Translation";
        public const string RESOURCES_LOCATION = "Project Settings/Translation";

        private static TranslationSettings _instance = null;
        public static TranslationSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<TranslationSettings>(RESOURCES_LOCATION);

#if UNITY_EDITOR
                    if (_instance == null)
                    {
                        var path = $"{Application.dataPath}/{DEFAULT_PROJECT_LOCATION}";
                        var directoryPath = Path.GetDirectoryName(path);

                        if (!Directory.Exists(directoryPath))
                            Directory.CreateDirectory(directoryPath);

                        _instance = CreateInstance<TranslationSettings>();
                        UnityEditor.AssetDatabase.CreateAsset(_instance, $"Assets/{DEFAULT_PROJECT_LOCATION}.asset");
                        UnityEditor.AssetDatabase.SaveAssets();
                        UnityEditor.AssetDatabase.Refresh();
                    }
#endif
                }

                return _instance;
            }
        }

        public Mapping.Mapping mapping;
        public TranslationSerialization serialization;
    }
}
