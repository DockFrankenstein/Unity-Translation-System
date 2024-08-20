using System.Collections;
using System.Collections.Generic;

namespace Translations
{
    public class RuntimeTranslation : IEnumerable<KeyValuePair<string, string>>
    {
        public string Name { get; set; }
        public string[] Authors { get; set; }
        Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();

        public bool TryGetValue(string tag, out string value) =>
            Values.TryGetValue(tag, out value);

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() =>
            Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public override string ToString() =>
            $"Runtime Translation \"{Name}\" by {string.Join(", ", Authors)}";
    }
}