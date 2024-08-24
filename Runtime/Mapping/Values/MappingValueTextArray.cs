namespace Translations.Mapping.Values
{
    public class MappingValueTextArray : MappingValue
    {
        public MappingValueTextArray() { }
        public MappingValueTextArray(string[] array)
        {
            this.array = array;
        }

        public override string Name => "Text Array";

        public override MappingValue Clone() =>
            new MappingValueTextArray(array);

        public string[] array;
    }
}
