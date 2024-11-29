using TMPro;
using Translations.Mapping.Values;
using UnityEngine;
using System.Collections.Generic;

namespace Translations
{
    public class TranslationTextLoaderTmp : MonoBehaviour
    {
        public TMP_Text target;
        public TranslatableText translatableText;

        [Space]
        public List<string> dynamicValues = new List<string>();

        private void Awake()
        {
            translatableText.OnLoad += TranslatableText_OnLoad;
            translatableText.Load();
        }

        private void TranslatableText_OnLoad(MappingValue val)
        {
            if (target == null) return;
            var textValue = (val as MappingValueText)?.GetValue(dynamicValues.ToArray()) ?? string.Empty;
            target.text = textValue;
        }
    }
}
