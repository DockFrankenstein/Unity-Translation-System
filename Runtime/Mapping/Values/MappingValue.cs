namespace Translations.Mapping.Values
{
    [System.Serializable]
    public abstract class MappingValue
    {
        public abstract string Name { get; }

        public string[] DynamicValueTags { get; set; }

        public abstract MappingValue Clone();
    }
}
