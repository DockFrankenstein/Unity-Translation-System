using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Translations.Mapping.Values;
using Translations.Serialization.Serializers;
using UnityEditor;
using UnityEngine;

namespace Translations.Serialization
{
    [CreateAssetMenu(fileName = "new Translation Serialization", menuName = "Translations/Serialization")]
    public class TranslationSerialization : ScriptableObject
    {
        [Header("Path")]
        public string path = "Translations";
        public string editorPath = "Editor Translations";

        [Header("Info")]
        public string infoFileName = "info.json";
        public string infoNameField = "Name";
        public string infoAuthorsField = "Authors";

        public List<TranslationSerializer> serializers = new List<TranslationSerializer>();

        public List<TranslationInfo> LoadedInfo { get; private set; } = new List<TranslationInfo>();

        public bool IsLoadingTranslationInfo { get; private set; }

        public void LoadListOfTranslations(string rootPath = null)
        {
            if (IsLoadingTranslationInfo)
                return;

            IsLoadingTranslationInfo = true;

            Task.Run(() => LoadListOfTranslationsAsync(rootPath));
        }

        public async Task LoadListOfTranslationsAsync(string rootPath = null)
        {
            if (rootPath == null)
                rootPath = $"{Application.dataPath}/{(Application.isEditor ? editorPath : path)}";

            Debug.Log($"Loading translations from '{rootPath}'...");

            LoadedInfo.Clear();

            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);

            var directories = Directory.GetDirectories(rootPath);

            foreach (var directory in directories)
            {
                try
                {
                    var infoPath = $"{directory}/{infoFileName}";

                    if (!File.Exists(infoPath))
                    {
                        //TODO: perhaps we should create a new translation here with a readme on how to work with it
                        continue;
                    }

                    var file = await File.ReadAllTextAsync(infoPath);
                    var json = JObject.Parse(file);

                    var info = new TranslationInfo();
                    info.Path = directory;
                    info.Name = (string)json[infoNameField] ?? "NO NAME";
                    info.Authors = ((JArray)json[infoAuthorsField])
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
            var excludedFiles = new string[] { infoNameField };
            var files = Directory.GetFiles(info.Path)
                .Where(x => !excludedFiles.Contains(Path.GetFileNameWithoutExtension(x)))
                .Where(x => Path.GetExtension(x) != ".meta");

            foreach (var path in files)
            {
                try
                {
                    var extension = Path.GetExtension(path).ToLower();

                    if (extension.StartsWith("."))
                        extension = extension.Substring(1, extension.Length - 1);

                    var serializer = serializers
                        .Where(x => x.FileExtensions.Contains(extension))
                        .FirstOrDefault();

                    if (serializer == null) continue;
                    serializer.Load(translation, File.ReadAllText(path));
                }
                catch (Exception e)
                {
                    Debug.LogError($"There was an error while loading a translation file at '{path}': {e}");
                }
            }

            return translation;
        }
    }
}
