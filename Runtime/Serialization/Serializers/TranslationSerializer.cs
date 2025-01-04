using System.Collections.Generic;
using Translations.Mapping.Values;
using UnityEngine;

namespace Translations.Serialization.Serializers
{
    public abstract class TranslationSerializer : ScriptableObject
    {
        public abstract string Id { get; }
        public abstract string[] FileExtensions { get; }

        public abstract void Deserialize(RuntimeTranslation translation, string txt);
        public abstract string Serialize(IEnumerable<KeyValuePair<string, MappingValue>> values);
    }
}