using System.Collections.Generic;
using Translations.Mapping.Values;

namespace Translations.Serialization.Serializers
{
    public class TranslationSerializerCsv : TranslationSerializer
    {
        public override string Id => "csv";

        public override string[] FileExtensions => new string[]
        {
            "csv",
        };

        public override void Deserialize(RuntimeTranslation translation, string txt)
        {
            var table = new CsvParser().Deserialize(txt);

            for (int r = 0; r < table.RowsCount; r++)
            {
                var key = table.GetCell(0, r);

                switch (translation.Values[key].value)
                {
                    case MappingValueText text:
                        text.content = table.GetCell(1, r);
                        break;
                    case MappingValueTextArray array:
                        table.SetCell(0, r, string.Empty);
                        var list = new List<string>();
                        
                        do
                        {
                            var val = table.GetCell(1, r);
                            if (!val.StartsWith("- ")) continue;
                            val = val.Substring(2, val.Length - 2);
                            
                            list.Add(val);
                            r++;
                        } while (string.IsNullOrWhiteSpace(table.GetCell(0, r)));
                        
                        r--;
                        array.content = list.ToArray();
                        break;
                }
            }
        }

        public override string Serialize(IEnumerable<KeyValuePair<string, MappingValue>> values)
        {
            var table = new Table2D();
            int i = 0;

            foreach (var v in values)
            {
                table.SetCell(0, i, v.Key);

                switch (v.Value)
                {
                    case MappingValueText text:
                        table.SetCell(1, i, text.content);
                        i++;
                        break;
                    case MappingValueTextArray array:
                        foreach (var item in array.content)
                        {
                            table.SetCell(1, i, $"- {item}");
                            i++;
                        }

                        if (array.content.Length == 0)
                            i++;
                        break;
                }
            }

            return new CsvParser().Serialize(table);
        }
    }
}