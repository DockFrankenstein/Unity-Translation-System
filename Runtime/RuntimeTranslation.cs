using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Translations.Mapping;
using Translations.Mapping.Values;

namespace Translations
{
    public class RuntimeTranslation : IEnumerable<RuntimeTranslation.Item>
    {
        public RuntimeTranslation(TranslationInfo info, Mapping.Mapping mapping) : this(info)
        {
            Values = mapping.GetAllItems()
                .ToDictionary(x => x.tag, x => new Item(x));
        }

        public RuntimeTranslation(TranslationInfo info)
        {
            Info = info;
        }

        public TranslationInfo Info { get; set; }
        public Dictionary<string, Item> Values { get; set; } = new Dictionary<string, Item>();

        public IEnumerator<Item> GetEnumerator() =>
            Values.Select(x => x.Value)
            .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public override string ToString() =>
            $"Runtime Translation \"{Info?.Name ?? "Unknown"}\" by {(Info == null ? "Unknown" : string.Join(", ", Info.Authors))}";

        public class Item
        {
            public Item() { }
            public Item(MappingItem mappingItem)
            {
                tag = mappingItem.tag;
                value = mappingItem.defaultValue?.Clone();
                this.mappingItem = mappingItem;
            }

            public string tag;
            public MappingValue value;
            public MappingItem mappingItem;
        }
    }
}