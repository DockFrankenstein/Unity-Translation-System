using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Translations.UI
{
    public class TranslationDropdownOption : MonoBehaviour
    {
        public Toggle toggle;
        public TMP_Text translationName;
        public TMP_Text authors;
        public TMP_Text path;

        private void Reset()
        {
            toggle = GetComponent<Toggle>();
        }

        public void LoadContent(TranslationInfo info)
        {
            LoadText(translationName, info.Name);
            LoadText(authors, string.Join(", ", info.Authors));
            LoadText(path, info.Path);
        }

        private void LoadText(TMP_Text text, string content)
        {
            if (text == null) return;
            text.text = content;
        }
    }
}