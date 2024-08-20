using UnityEngine;
using UnityEditor;
using System;

namespace Translations.Editor
{
    public class AssetEditorToolbar<T, TAsset> where T : AssetEditorWindow<T, TAsset>
        where TAsset : ScriptableObject
    {
        public T window;

        public AssetEditorToolbar(T window)
        {
            this.window = window;
        }

        public void OnGUI()
        {
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                OnLeftGUI();
                GUILayout.FlexibleSpace();
                OnRightGUI();
            }
        }

        protected virtual void OnLeftGUI()
        {

        }

        protected virtual void OnRightGUI()
        {

        }

        protected static void DisplayMenu(string buttonText, ref Rect rect, Action<GenericMenu> menuFunction)
        {
            bool openMenu = GUILayout.Button(buttonText, EditorStyles.toolbarDropDown);

            //Calculating rect
            //This is a really janky solution, but sometimes GetLastRect returns 0 0
            //so in order to create the generic menu in the correct position, the rect
            //needs to be saved if the x position is not equal 0
            Rect r = GUILayoutUtility.GetLastRect();

            if (rect == null)
                rect = r;

            if (r.x != 0)
                rect = r;


            if (!openMenu) return;
            GenericMenu menu = new GenericMenu();
            menuFunction.Invoke(menu);
            menu.DropDown(rect);
        }

        protected void GUISaveButton()
        {
            if (GUILayout.Button("Save", EditorStyles.toolbarButton))
                window.SaveChanges();
        }

        protected void GUIAutoSaveButton()
        {
            if (GUILayout.Toggle(window.asset != null && window.Prefs_AutoSave, $"Auto Save", EditorStyles.toolbarButton) != window.Prefs_AutoSave)
                window.Prefs_AutoSave = !window.Prefs_AutoSave;
        }
    }
}