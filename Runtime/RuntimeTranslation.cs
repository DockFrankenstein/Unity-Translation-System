using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Translations.Mapping;

namespace Translations
{
    public class RuntimeTranslation : IEnumerable<RuntimeTranslation.Item>
    {
        public string Name { get; set; }
        public string[] Authors { get; set; }
        Dictionary<string, Item> Values { get; set; } = new Dictionary<string, Item>();

        public IEnumerator<Item> GetEnumerator() =>
            Values.Select(x => x.Value)
            .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public override string ToString() =>
            $"Runtime Translation \"{Name}\" by {string.Join(", ", Authors)}";

        public class Item
        {
            public string tag;
            public string text;
            public TranslationMappingItem mappingItem;
        }
    }
}