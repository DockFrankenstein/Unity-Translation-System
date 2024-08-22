using System;
using Translations.Serialization;
using UnityEngine;

namespace Translations
{
    public static class TranslationManager
    {
        public static TranslationSerializationManager Serializer { get; set; } = new TranslationSerializationManager();

        public static RuntimeTranslation LoadedTranslation { get; private set; }

        public static event Action OnLoad;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Initialize()
        {
            var translationsPath = Application.isEditor ?
                TranslationSettings.Instance.editorTranslationsPath :
                TranslationSettings.Instance.translationsPath;

            Serializer.LoadListOfTranslations($"{Application.dataPath}/{translationsPath}");
        }

        public static void LoadTranslation(RuntimeTranslation translation)
        {
            LoadedTranslation = translation;
            OnLoad?.Invoke();
        }

        public static string GetText(string tag)
        {
            return tag;
        }

    }
}