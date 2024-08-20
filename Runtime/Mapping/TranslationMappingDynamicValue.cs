using System;

namespace Translations.Mapping
{
    [Serializable]
    public class TranslationMappingDynamicValue
    {
        public TranslationMappingDynamicValue() { }
        public TranslationMappingDynamicValue(string tag)
        {
            this.tag = tag;
        }

        public string tag;
        public string note;
    }
}