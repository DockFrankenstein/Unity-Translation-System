using UnityEngine;

namespace Translations.Mapping.Values
{
    [System.Serializable]
    public class MappingValueText : MappingValue
    {
        public MappingValueText() { }
        public MappingValueText(string content)
        {
            this.content = content;
        }

        public override string Name => "Text";

        public override MappingValue Clone() =>
            new MappingValueText(content)
            {
                DynamicValueTags = DynamicValueTags
            };

        public string GetValue(params string[] dynamicValues)
        {
            var result = content;
            var dynMin = Mathf.Min(DynamicValueTags.Length, dynamicValues.Length);
            for (int i = 0; i < dynMin; i++)
                result = result.Replace($"[{DynamicValueTags[i]}]", dynamicValues[i]);

            return result;
        }

        public string content;
    }
}
