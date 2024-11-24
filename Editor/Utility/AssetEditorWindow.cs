using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using System.IO;

namespace Translations.Editor
{
    public abstract class AssetEditorWindow<TWindow, TAsset> : EditorWindow where TWindow : AssetEditorWindow<TWindow, TAsset>
        where TAsset : ScriptableObject
    {
        [SerializeField] Texture2D icon;

        [SerializeField] public TAsset asset;
        [NonSerialized] protected bool initialized;

        private static Dictionary<TAsset, List<TWindow>> _openedWindows = new Dictionary<TAsset, List<TWindow>>();

        /// <summary>Title of the window that will be displayed on the top.</summary>
        public abstract string WindowTitle { get; }

        /// <summary>Prefix for every key used to save SessionState or EditorPrefs. Keep the prefix lowercase for consistency.</summary>
        public abstract string PrefsKeyPrefix { get; }
        public virtual Vector2 MinWindowSize => new Vector2(512f, 256f);
        public virtual Vector2 MaxWindowSize => new Vector2(4000f, 4000f);

        #region Creating
        public static TWindow CreateWindow()
        {
            var window = CreateWindow<TWindow>();

            window.minSize = window.MinWindowSize;
            window.maxSize = window.MaxWindowSize;

            window.UpdateWindowTitle();
            window.OnCreateWindow();
            window.Show();

            return window;
        }

        public static void OpenAsset(TAsset asset)
        {
            if (_openedWindows.ContainsKey(asset) && _openedWindows[asset].Count > 0)
            {
                _openedWindows[asset][0].Show();
                return;
            }

            var window = CreateWindow();
            window.asset = asset;
            window.UpdateWindowTitle(); 
        }

        public void UpdateWindowTitle()
        {
            titleContent = new GUIContent(asset ? asset.name : WindowTitle, icon);
        }

        protected virtual void OnCreateWindow()
        {

        }
        #endregion

        #region Initialization
        protected void InitializeIfRequired()
        {
            if (!initialized)
                Initialize();
        }

        protected virtual void Initialize()
        {
            initialized = true;
            EnsureWindowIsRegistered();
        }
        #endregion

        protected virtual void OnGUI()
        {
            InitializeIfRequired();

            if (IsDirty && Prefs_AutoSave)
                SaveChanges();
        }

        protected virtual void OnDestroy()
        {
            if (asset != null &&
                _openedWindows.ContainsKey(asset) &&
                _openedWindows[asset].Contains(this as TWindow))
                _openedWindows[asset].Remove(this as TWindow);
        }

        private void EnsureWindowIsRegistered()
        {
            if (asset == null)
                return;

            if (!_openedWindows.ContainsKey(asset))
                _openedWindows.Add(asset, new List<TWindow>());

            _openedWindows[asset].Add(this as TWindow);
        }

        protected static TWindow[] GetWindows() =>
            Resources.FindObjectsOfTypeAll<TWindow>();

        protected static TWindow GetWindow() =>
            GetWindows()
                .FirstOrDefault();

        #region Saving
        private string prefsKey_autoSave => $"{PrefsKeyPrefix}_autosave";

        private bool? prefs_autoSave = null;
        public bool Prefs_AutoSave
        {
            get
            {
                if (prefs_autoSave == null)
                    prefs_autoSave = EditorPrefs.GetBool(prefsKey_autoSave, true);

                return prefs_autoSave ?? false;
            }
            set
            {
                if (value == Prefs_AutoSave)
                    return;

                EditorPrefs.SetBool(prefsKey_autoSave, value);

                if (IsDirty)
                    SaveChanges();

                prefs_autoSave = value;
            }
        }

        private bool? _isDirty = null;
        public bool IsDirty
        {
            get
            {
                if (asset == null)
                    return false;

                if (_isDirty == null)
                    _isDirty = EditorUtility.GetDirtyCount(asset) != 0;

                return _isDirty ?? false;
            }
        }

        public virtual void SetAssetDirty()
        {
            hasUnsavedChanges = true;
            _isDirty = true;
            EditorUtility.SetDirty(asset);
            UpdateWindowTitle();
        }

        public override void SaveChanges()
        {
            _isDirty = false;
            hasUnsavedChanges = false;

            var json = JsonUtility.ToJson(asset, true);

            var relativePath = AssetDatabase.GetAssetPath(asset);
            var path = $"{Application.dataPath}/{relativePath.Remove(0, 7)}";
            File.WriteAllText(path, json);

            UpdateWindowTitle();
        }

        public override void DiscardChanges()
        {
            _isDirty = false;
            hasUnsavedChanges = false;
            var relativePath = AssetDatabase.GetAssetPath(asset);

            AssetDatabase.ImportAsset(relativePath);

            UpdateWindowTitle();
        }
        #endregion

        #region GUI
        protected void DrawTreeView(TreeView tree)
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            Rect rect = GUILayoutUtility.GetLastRect();
            tree?.OnGUI(rect);
        }
        #endregion
    }
}