using System;
using Translations.Mapping.Values;
using UnityEngine;

namespace Translations
{
    /// <summary>Handles loading of a specific text string from the translation. Use this to assign tags in the inspector.</summary>
    [Serializable]
    public class TranslatableText
    {
        [SerializeField] string tag;
        public event Action<MappingValue> OnLoad;

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
        public MappingValue Load(bool silent = false)
        {
            if (!reigsteredToManager)
            {
                TranslationManager.OnLoad += () => Load(); 
                reigsteredToManager = true;
            }

            var value = TranslationManager.GetValue(tag);

            if (!silent)
                OnLoad?.Invoke(value);
            
            return value;
        }

        /// <summary>Loads translation text.</summary>
        /// <typeparam name="T">Type of the value to load.</typeparam>
        /// <param name="silent">When true, <see cref="OnLoad"/> will not be invoked.</param>
        /// <returns>Returns the loaded text.</returns>
        public T Load<T>(bool silent = false) where T : MappingValue =>
            Load(silent) as T;
    }
}