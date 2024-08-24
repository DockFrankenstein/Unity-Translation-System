using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Translations.Mapping;
using UnityEngine;

namespace Translations.Serialization
{
    public class TranslationSerializationManager
    {
        public List<TranslationInfo> LoadedInfo { get; private set; } = new List<TranslationInfo>();

        public bool IsLoadingTranslationInfo { get; private set; }

        public void LoadListOfTranslations(string rootPath)
        {
            if (IsLoadingTranslationInfo)
                return;

            IsLoadingTranslationInfo = true;

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
                    info.Path = directory;
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

            IsLoadingTranslationInfo = false;
            Debug.Log("Finished loading translations!");
        }

        public RuntimeTranslation LoadTranslation(TranslationInfo info)
        {
            var settings = TranslationSettings.Instance;

            if (!Directory.Exists(info.Path))
            {
                Debug.LogError("Translation could not be loaded: the specified directory does not exist!");
                return null;
            }

            var translation = new RuntimeTranslation(info, settings.mapping);
            var excludedFiles = new string[] { settings.infoNameField };
            var files = Directory.GetFiles(info.Path)
                .Where(x => !excludedFiles.Contains(Path.GetFileNameWithoutExtension(x)))
                .Where(x => Path.GetExtension(x) != ".meta");

            foreach (var item in files)
            {
                LoadFile(translation, item);
            }

            return translation;
        }

        void LoadFile(RuntimeTranslation translation, string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();

            var file = File.ReadAllText(filePath);

            switch (extension)
            {
                case ".json":
                    var json = JObject.Parse(file);

                    foreach (var item in json)
                    {
                        if (!translation.Values.ContainsKey(item.Key)) continue;
                        var val = (string)item.Value;

                        if (val != null)
                            translation.Values[item.Key].value = val;
                    }

                    break;
            }
        }
    }
}
