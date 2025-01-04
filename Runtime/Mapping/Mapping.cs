using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Translations.Mapping
{
    public class Mapping : ScriptableObject
    {
        public const string EXTENSION = "transmap";

        public List<MappingGroup> groups = new List<MappingGroup>();

        public IEnumerable<MappingItem> GetAllItems() =>
            groups.SelectMany(x => x.items);

        public MappingItem FindItem(string tag) =>
            groups.SelectMany(x => x.items)
            .Where(x => x.tag == tag)
            .FirstOrDefault();

        public bool genLowercase = true;
        public DefaultGenerationBehaviour genBehaviour = DefaultGenerationBehaviour.ByGroup;
        public string genSingleFileName = "main";

        public enum DefaultGenerationBehaviour
        {
            SingleFile,
            ByGroup,
        }
    }
}