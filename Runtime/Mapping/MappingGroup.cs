using System;
using System.Collections.Generic;

namespace Translations.Mapping
{
    [Serializable]
    public class MappingGroup
    {
        public MappingGroup() { }
        public MappingGroup(string name)
        {
            this.name = name;
        }

        public string name;
        public List<MappingItem> items = new List<MappingItem>();
    }
}
