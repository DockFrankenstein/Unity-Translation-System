using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Translations.Mapping.Values;

namespace Translations.Serialization.Serializers
{
    [System.Serializable]
    public class TranslationSerializerJson : TranslationSerializer
    {
        public override string Id => "json";

        public override string[] FileExtensions => new string[]
        {
            "json"
        };

        public override void Deserialize(RuntimeTranslation translation, string txt)
        {
            var json = JObject.Parse(txt);

            foreach (var item in json)
            {
                if (!translation.Values.TryGetValue(item.Key, out var translationItem)) continue;

                switch (translationItem.value)
                {
                    case MappingValueText valueText:
                        if (item.Value.Type == JTokenType.String)
                        {
                            valueText.DynamicValueTags = translationItem.mappingItem.GetDynamicValueTags();
                            valueText.content = (string)item.Value;
                        }
                        break;
                    case MappingValueTextArray valueArray:
                        if (item.Value.Type == JTokenType.Array)
                        {
                            valueArray.DynamicValueTags = translationItem.mappingItem.GetDynamicValueTags();
                            valueArray.content = item.Value
                                .Where(x => x.Type == JTokenType.String)
                                .Select(x => (string)x)
                                .ToArray();
                        }
                        break;
                }
            }
        }

        public override string Serialize(IEnumerable<KeyValuePair<string, MappingValue>> values)
        {
            var json = new JObject();

            foreach (var item in values)
            {
                switch (item.Value)
                {
                    case MappingValueText text:
                        json.Add(item.Key, new JValue(text.content));
                        break;
                    case MappingValueTextArray array:
                        json.Add(item.Key, new JArray(array.content.Select(x => new JValue(x))));
                        break;
                }
            }

            return json.ToString();
        }
    }
}
