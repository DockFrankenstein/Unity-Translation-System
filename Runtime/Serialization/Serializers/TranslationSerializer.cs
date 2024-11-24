using UnityEngine;

namespace Translations.Serialization.Serializers
{
    public abstract class TranslationSerializer : ScriptableObject
    {
        public abstract string[] FileExtensions { get; }

        public abstract void Load(RuntimeTranslation translation, string txt);
    }
}