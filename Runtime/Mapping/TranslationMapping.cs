using System.Collections.Generic;
using UnityEngine;

namespace Translations.Mapping
{
    public class TranslationMapping : ScriptableObject
    {
        public const string EXTENSION = "transmap";

        public List<TranslationMappingGroup> groups = new List<TranslationMappingGroup>();
    }
}