namespace Translations.Mapping.Values
{
    [System.Serializable]
    public class MappingValueText : TranslationMappingValue<string>
    {
        public override string Name => "Text";

        public bool test;
    }
}
