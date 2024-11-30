using UnityEngine;

namespace Translations.Mapping.Values
{
    public class MappingValueTextArray : MappingValue
    {
        public MappingValueTextArray() { }
        public MappingValueTextArray(string[] content)
        {
            this.content = content;
        }

        public override string Name => "Text Array";

        public override MappingValue Clone() =>
            new MappingValueTextArray(content)
            {
                DynamicValueTags = DynamicValueTags
            };

        public string[] GetValue(string[] dynamicValues)
        {
            var result = content;

            var dynMin = Mathf.Min(dynamicValues.Length, DynamicValueTags.Length);
            for (int i = 0; i < result.Length; i++)
                for (int dynI = 0; dynI < dynMin; dynI++)
                    result[i] = result[i].Replace($"[{DynamicValueTags[dynI]}]", dynamicValues[i]);

            return result;
        }

        public string[] content;
    }
}
