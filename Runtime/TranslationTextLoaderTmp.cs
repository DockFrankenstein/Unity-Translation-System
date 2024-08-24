using TMPro;
using Translations.Mapping.Values;
using UnityEngine;

namespace Translations
{
    public class TranslationTextLoaderTmp : MonoBehaviour
    {
        public TMP_Text target;
        public TranslatableText translatableText;

        private void Awake()
        {
            translatableText.OnLoad += TranslatableText_OnLoad;

            translatableText.Load();
        }

        private void TranslatableText_OnLoad(MappingValue val)
        {
            if (target != null)
                target.text = (val as MappingValueText)?.value ?? string.Empty;
        }
    }
}
