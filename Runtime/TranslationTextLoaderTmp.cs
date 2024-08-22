﻿using TMPro;
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

        private void TranslatableText_OnLoad(string txt)
        {
            if (target != null)
                target.text = txt;
        }
    }
}
