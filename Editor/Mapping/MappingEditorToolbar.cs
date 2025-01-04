using System.IO;
using UnityEditor;
using UnityEngine;

namespace Translations.Editor.Mapping
{
    internal class MappingEditorToolbar : AssetEditorToolbar<MappingWindow, Translations.Mapping.Mapping>
    {
        public MappingEditorToolbar(MappingWindow window) : base(window)
        {
        }

        Rect r_add;
        Rect r_gen;

        protected override void OnLeftGUI()
        {
            base.OnLeftGUI();
            DisplayMenu("Add", ref r_add, menu =>
            {
                menu.AddItem("Group", false, () => window.CreateNewGroup());
                menu.AddItem("Item", false, () => window.CreateNewItem());
                menu.AddItem("Dynamic Value", false, () => window.CreateNewDynamicValue());
            });

            DisplayMenu("Generate Default", ref r_gen, menu =>
            {
                menu.AddItem("Generate Default", false, () =>
                {
                    var serializer = TranslationSettings.Instance.serialization;
                    var path = serializer.DefaultTranslationPath;
                    foreach (var file in Directory.GetFiles(path))
                        if (Path.GetFileName(file) != serializer.infoFileName)
                            File.Delete(file);
                    
                    foreach (var dir in Directory.GetDirectories(path))
                        Directory.Delete(dir, true);

                    GenerateTranslation(TranslationSettings.Instance.serialization.DefaultTranslationPath);

                });

                menu.AddItem("Generate Template", false, () =>
                {
                    var path = EditorUtility.OpenFolderPanel("Select Target Folder", Application.dataPath, "Template");

                    if (!Directory.Exists(path)) return;
                    GenerateTranslation(path);
                });

                menu.AddSeparator("");
                menu.AddItem("Generation Settings", false, () => window.inspector.SelectedObjects = new string[] { "gen" });
            });
        }

        protected override void OnRightGUI()
        {
            base.OnRightGUI();
            GUIAutoSaveButton();
            GUISaveButton();
            EditorGUILayout.Space();
        }

        private void GenerateTranslation(string path)
        {
            var serializer = TranslationSettings.Instance.serialization;
            serializer.GenerateTranslation(path, window.asset);

            var infoPath = $"{path}/{serializer.infoFileName}";

            if (!File.Exists(infoPath))
            {
                File.WriteAllText(infoPath,  serializer.WriteInfoToText(new TranslationInfo()
                {
                    Name = "Template",
                    Authors = new string[]
                    {
                        "Insert author here",
                    }
                }));
            }
            
            AssetDatabase.Refresh();
        }
    }
}