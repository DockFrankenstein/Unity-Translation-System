using System.Collections.Generic;
using Translations.Mapping.Values;
using UnityEngine;
using UnityEngine.UI;

namespace Translations.UI
{
    public class TranslationTextLoader : MonoBehaviour
    {
        public Text target;
        public TranslatableText translatableText;

        [Space]
        public List<string> dynamicValues = new List<string>();

        private void Awake()
        {
            translatableText.OnLoad += TranslatableText_OnLoad;
            translatableText.Load();
        }

        private void Reset()
        {
            target = GetComponent<Text>();
        }

        private void TranslatableText_OnLoad(MappingValue val)
        {
            if (target == null) return;
            var textValue = (val as MappingValueText)?.GetValue(dynamicValues.ToArray()) ?? string.Empty;
            target.text = textValue;
        }
    }
}