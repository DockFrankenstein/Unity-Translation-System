using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Translations.Serialization.Serializers;
using UnityEngine;

namespace Translations.Serialization
{
    [CreateAssetMenu(fileName = "new Translation Serialization", menuName = "Translations/Serialization")]
    public class TranslationSerialization : ScriptableObject
    {
        [Header("Path")]
        [SerializeField] string path = "Translations";
        [SerializeField] string editorPath = "Editor Translations";

        [Header("Default")]
        [Tooltip("Path inside of the translation folder for the default translation")]
        [SerializeField] string defaultTranslationPath = "en";

        [Header("Info")]
        public string infoFileName = "info.json";
        public string infoNameField = "Name";
        public string infoAuthorsField = "Authors";

        public event Action OnStartLoadingInfoList;

        public List<TranslationSerializer> serializers = new List<TranslationSerializer>();

        public string TranslationRootPath =>
            $"{Application.dataPath}/{(Application.isEditor ? editorPath : path)}";

        public string DefaultTranslationPath =>
            $"{TranslationRootPath}/{defaultTranslationPath}";

        public List<TranslationInfo> LoadedInfo { get; private set; } = new List<TranslationInfo>();

        public bool IsLoadingTranslationInfo { get; private set; }

        public TranslationInfo GetInfo(string path)
        {
            path = Path.GetFullPath(path);
            return LoadedInfo.Where(x => x.Path == path)
                .FirstOrDefault();
        }

        public TranslationInfo LoadInfo(string path)
        {
            var info = LoadInfoFromText(File.ReadAllText(path));
            info.Path = Path.GetFullPath(Path.GetDirectoryName(path));
            return info;
        }

        public async Task<TranslationInfo> LoadInfoAsync(string path)
        {
            var info = LoadInfoFromText(await File.ReadAllTextAsync(path));
            info.Path = Path.GetDirectoryName(path);
            return info;
        }

        public TranslationInfo LoadInfoFromText(string txt)
        {
            var json = JObject.Parse(txt);

            var info = new TranslationInfo();
            info.Name = (string)json[infoNameField] ?? "NO NAME";
            info.Authors = ((JArray)json[infoAuthorsField])?
                .ToObject<string[]>() ?? Array.Empty<string>();

            return info;
        }

        public void LoadListOfTranslations(string rootPath = null)
        {
            if (IsLoadingTranslationInfo)
                return;

            IsLoadingTranslationInfo = true;

            Task.Run(() => LoadListOfTranslationsAsync(rootPath));
        
        }

        public async Task LoadListOfTranslationsAsync(string rootPath = null)
        {
            OnStartLoadingInfoList?.Invoke();

            if (rootPath == null)
                rootPath = TranslationRootPath;

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

                    var info = await LoadInfoAsync(infoPath);

                    LoadedInfo.Add(info);

                    Debug.Log($"Loaded: {info}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"There was an error while loading translation info for \"{Path.GetFileName(directory)}\": {e}");
                }
            }

            //TODO: remove this
            await Task.Delay(3000);

            IsLoadingTranslationInfo = false;
            Debug.Log("Finished loading translations!");
        }

        public RuntimeTranslation LoadTranslation(TranslationInfo info)
        {
            var settings = TranslationSettings.Instance;

            if (!Directory.Exists(info.Path))
            {
                Debug.LogError($"Translation could not be loaded: the specified directory '{info.Path}' does not exist!");
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
