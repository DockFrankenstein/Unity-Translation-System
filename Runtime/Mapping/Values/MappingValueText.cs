namespace Translations.Mapping.Values
{
    [System.Serializable]
    public class MappingValueText : MappingValue
    {
        public MappingValueText() { }
        public MappingValueText(string value)
        {
            this.value = value;
        }

        public override string Name => "Text";
        public override MappingValue Clone() =>
            new MappingValueText(value);

        public string value;
    }
}
