using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Translations.Mapping
{
    public class TranslationMapping : ScriptableObject
    {
        public const string EXTENSION = "transmap";

        public List<TranslationMappingGroup> groups = new List<TranslationMappingGroup>();

        public IEnumerable<TranslationMappingItem> GetAllItems() =>
            groups.SelectMany(x => x.items);

        public TranslationMappingItem FindItem(string tag) =>
            groups.SelectMany(x => x.items)
            .Where(x => x.tag == tag)
            .FirstOrDefault();
    }
}