using System;

namespace Translations.Mapping
{
    [Serializable]
    public class MappingDynamicValue
    {
        public MappingDynamicValue() { }
        public MappingDynamicValue(string tag)
        {
            this.tag = tag;
        }

        public string tag;
        public string note;
    }
}