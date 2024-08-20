using System;
using System.Collections.Generic;

namespace Translations.Mapping
{
    [Serializable]
    public class TranslationMappingItem
    {
        public TranslationMappingItem() { }
        public TranslationMappingItem(string tag)
        {
            this.tag = tag;
        }

        public string tag;
        public string defaultValue;

        public List<TranslationMappingDynamicValue> dynamicValues = new List<TranslationMappingDynamicValue>();
    }
}