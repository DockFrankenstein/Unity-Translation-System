namespace Translations.Mapping.Values
{
    [System.Serializable]
    public abstract class MappingValue
    {
        public abstract string Name { get; }
        public abstract object ValueObject { get; set; }
    }

    [System.Serializable]
    public abstract class TranslationMappingValue<T> : MappingValue
    {
        public override object ValueObject
        {
            get => value;
            set => this.value = (T)value;
        }

        public T value;
    }
}