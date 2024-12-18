﻿namespace Translations
{
    public class TranslationInfo
    {
        public string Name { get; set; }
        public string[] Authors { get; set; }
        public string Path { get; set; }

        public override string ToString() =>
            $"Translation Info '{Name}', made by {string.Join(", ", Authors ?? new string[0])}";
    }
}