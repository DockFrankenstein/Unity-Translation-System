using System;
using UnityEngine;

namespace Translations
{
    /// <summary>Handles loading of a specific text string from the translation. Use this to assign tags in the inspector.</summary>
    [Serializable]
    public class TranslatableText
    {
        [SerializeField] string tag;
        public event Action<string> OnLoad;

        /// <summary>Tag that will be used for identifying the correct text in a translation.</summary>
        public string Tag
        {
            get => tag;
            set => tag = value;
        }

        bool reigsteredToManager;

        /// <summary>Loads translation text.</summary>
        /// <param name="silent">When true, <see cref="OnLoad"/> will not be invoked.</param>
        /// <returns>Returns the loaded text.</returns>
        public string Load(bool silent = false)
        {
            if (!reigsteredToManager)
            {
                TranslationManager.OnLoad += () => Load(); 
                reigsteredToManager = true;
            }

            var value = TranslationManager.GetText(tag);

            if (!silent)
                OnLoad?.Invoke(value);
            
            return value;
        }
    }
}