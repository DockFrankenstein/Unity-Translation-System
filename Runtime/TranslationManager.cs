using System;

namespace Translations
{
    public static class TranslationManager
    {
        public static event Action OnLoad;

        public static string GetText(string tag)
        {
            return tag;
        }

    }
}