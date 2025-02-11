using System;
using Translations.Mapping.Values;
using Translations.Serialization;
using UnityEngine;

namespace Translations
{
    public static class TranslationManager
    {
        public static TranslationSerialization Serializer =>
            TranslationSettings.Instance.serialization;

        public static RuntimeTranslation LoadedTranslation { get; private set; }

        public static event Action OnLoad;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Initialize()
        {
            var defaultTransInfo = new TranslationInfo()
            {
                Path = Serializer.DefaultTranslationPath,
            };

            LoadTranslation(new RuntimeTranslation(defaultTransInfo, TranslationSettings.Instance.mapping));

            if (Serializer != null)
            {
                Serializer.IsLoadingTranslationInfo = false;

                try
                {
                    var info = Serializer.LoadInfo($"{defaultTransInfo.Path}/{Serializer.infoFileName}");
                    var trans = Serializer.LoadTranslation(info);
                    LoadTranslation(trans);
                }
                catch (Exception e)
                {
                    Debug.LogError($"There was an error loading default translation, fallback to mapping asset: {e}");
                }

                Serializer.LoadListOfTranslations();
            }
        }

        public static void LoadTranslation(RuntimeTranslation translation)
        {
            LoadedTranslation = translation;
            Debug.Log($"Loaded translation {translation.Info.Name}");
            OnLoad?.Invoke();
        }

        public static MappingValue GetValue(string tag) =>
            LoadedTranslation?.Values?.TryGetValue(tag, out var val) == true ?
            val.value :
            null;
    }
}
