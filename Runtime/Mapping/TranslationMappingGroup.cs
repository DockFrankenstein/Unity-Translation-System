using System;
using System.Collections.Generic;

namespace Translations.Mapping
{
    [Serializable]
    public class TranslationMappingGroup
    {
        public TranslationMappingGroup() { }
        public TranslationMappingGroup(string name)
        {
            this.name = name;
        }

        public string name;
        public List<TranslationMappingItem> items = new List<TranslationMappingItem>();
    }
}
