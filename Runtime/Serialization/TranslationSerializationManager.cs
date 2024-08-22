using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Translations.Serialization
{
    public class TranslationSerializationManager
    {
        public List<TranslationInfo> LoadedInfo { get; private set; } = new List<TranslationInfo>();

        public bool IsLoadingTranslations { get; private set; }

        public void LoadListOfTranslations(string rootPath)
        {
            if (IsLoadingTranslations)
                return;

            IsLoadingTranslations = true;

            Task.Run(() => LoadListOfTranslationsAsync(rootPath));
        }

        public async Task LoadListOfTranslationsAsync(string rootPath)
        {
            var settings = TranslationSettings.Instance;

            Debug.Log($"Loading translations from '{rootPath}'...");

            LoadedInfo.Clear();

            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);

            var directories = Directory.GetDirectories(rootPath);

            foreach (var directory in directories)
            {
                try
                {
                    var infoPath = $"{directory}/{settings.infoFileName}";

                    if (!File.Exists(infoPath))
                    {
                        //TODO: perhaps we should create a new translation here with a readme on how to work with it
                        continue;
                    }

                    var file = await File.ReadAllTextAsync(infoPath);
                    var json = JObject.Parse(file);

                    var info = new TranslationInfo();
                    info.Name = (string)json[settings.infoNameField] ?? "NO NAME";
                    info.Authors = ((JArray)json[settings.infoAuthorsField])
                        .ToObject<string[]>() ?? Array.Empty<string>();

                    LoadedInfo.Add(info);

                    Debug.Log($"Loaded: {info}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"There was an error while loading translation info for \"{Path.GetFileName(directory)}\": {e}");
                }
            }

            IsLoadingTranslations = false;
            Debug.Log("Finished loading translations!");
        }
    }
}
