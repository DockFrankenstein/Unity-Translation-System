using System;
using Translations.Editor.Mapping;
using Translations.Mapping;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Translations.Editor.Drawers
{
    public class TranslatableTextExplorer : PopupWindowContent
    {
        public static void Open(Rect rect, string defaultValue, Action<string> onSelect)
        {
            var window = new TranslatableTextExplorer();
            var size = new Vector2(Mathf.Clamp(rect.width, 200, 300f), 250f);

            window._size = size;
            window.defaultValue = defaultValue;
            window.OnSelect = onSelect;

            PopupWindow.Show(rect, window);
        }

        TranslationMappingTree tree;
        TreeViewState treeState;

        Vector2 _size;
        string defaultValue;

        TranslationMapping mapping;

        public Action<string> OnSelect;

        public override Vector2 GetWindowSize() =>
            _size;

        public override void OnOpen()
        {
            base.OnOpen();

            mapping = TranslationSettings.Instance.mapping;

            if (mapping != null)
            {
                treeState = new TreeViewState();
                tree = new TranslationMappingTree(treeState, mapping)
                {
                    ReadOnly = true,
                };

                tree.OnDoubleClicked += obj =>
                {
                    if (obj is TranslationMappingItem item)
                        SelectAndClose(item);
                };

                tree.Reload();
                tree.ExpandAll();

                var item = mapping.FindItem(defaultValue);

                if (item != null)
                {
                    tree.MakeVisible(item);
                    tree.SetSelection(new int[] { item.GetHashCode() });
                }

                tree.SetFocus();
            }
        }

        bool init = false;

        public override void OnGUI(Rect rect)
        {
            if (!init)
            {
                init = true;
                tree?.SetFocus();
            }

            Rect searchRect = rect.ResizeToTop(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2f)
                .Border(EditorGUIUtility.standardVerticalSpacing);
            Rect searchBarRect = searchRect
                .BorderRight(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            Rect addButtonRect = searchRect
                .ResizeToRight(EditorGUIUtility.singleLineHeight);

            Rect selectRect = rect.ResizeToBottom(24f);
            Rect treeRect = rect.BorderBottom(selectRect.height)
                .BorderTop(searchRect.height)
                .BorderTop(EditorGUIUtility.standardVerticalSpacing);

            bool selectDisabled = false;

            using (new EditorGUI.DisabledGroupScope(mapping == null))
            {
                tree.BetterSearchString = EditorGUI.TextField(searchBarRect, tree.BetterSearchString);
                if (GUI.Button(addButtonRect, ToolGuiUtility.PlusIcon, EditorStyles.label))
                { }
            }

            switch (mapping == null)
            {
                case true:
                    EditorGUI.LabelField(treeRect, "Please Assign A Translation Mapping In \"Project Settings/Translation System\"", Styles.ErrorLabel);
                    selectDisabled = true;
                    break;
                case false:
                    tree.OnGUI(treeRect);

                    var selection = tree.GetSelectedObjects();
                    selectDisabled = selection.Count != 1 ||
                        !(selection[0] is TranslationMappingItem);

                    if (Event.current.keyCode == KeyCode.Return && !selectDisabled)
                        SelectAndClose(tree.GetSelectedItem().tag);
                    break;
            }

            using (new EditorGUI.DisabledGroupScope(selectDisabled))
            {
                if (GUI.Button(selectRect, "Select"))
                    SelectAndClose(tree.GetSelectedItem().tag);
            }
        }

        void SelectAndClose(TranslationMappingItem item) =>
            SelectAndClose(item.tag);

        void SelectAndClose(string tag)
        {
            OnSelect?.Invoke(tag);
            editorWindow.Close();
        }

        static class Styles
        {
            public static GUIStyle ErrorLabel =>
                new GUIStyle()
                {
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true,
                    fontSize = 18,
                    normal = new GUIStyleState()
                    {
                        textColor = Color.red,
                    }
                };
        }
    }
}
