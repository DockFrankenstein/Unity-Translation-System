﻿using UnityEditor;
using UnityEngine;
using Translations.Mapping;

namespace Translations.Editor.Mapping
{
    internal class TranslationMappingEditorToolbar : AssetEditorToolbar<TranslationMappingWindow, TranslationMapping>
    {
        public TranslationMappingEditorToolbar(TranslationMappingWindow window) : base(window)
        {
        }

        Rect r_add;

        protected override void OnLeftGUI()
        {
            base.OnLeftGUI();
            DisplayMenu("Add", ref r_add, menu =>
            {
                menu.AddItem("Group", false, () => window.CreateNewGroup());
                menu.AddItem("Item", false, () => window.CreateNewItem());
                menu.AddItem("Dynamic Value", false, () => window.CreateNewDynamicValue());
            });
        }

        protected override void OnRightGUI()
        {
            base.OnRightGUI();
            GUIAutoSaveButton();
            GUISaveButton();
            EditorGUILayout.Space();
        }
    }
}