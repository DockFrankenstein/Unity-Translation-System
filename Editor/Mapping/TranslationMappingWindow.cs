using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Translations.Mapping;

namespace Translations.Editor.Mapping
{
    internal class TranslationMappingWindow : AssetEditorWindow<TranslationMappingWindow, TranslationMapping>
    {
        public override string WindowTitle => "Translation Mapping Editor";

        public override string PrefsKeyPrefix => "transmap";

        public TranslationMappingEditorToolbar toolbar;
        public TranslationMappingTree tree;
        public TreeViewState treeState;

        protected override void Initialize()
        {
            base.Initialize();
            toolbar = new TranslationMappingEditorToolbar(this);

            if (treeState == null)
                treeState = new TreeViewState();

            tree = new TranslationMappingTree(treeState, asset);
            tree.OnAssetModified += SetAssetDirty;

            tree.Reload();
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            toolbar.OnGUI();
            DrawTreeView(tree);
        }

        public override void Save()
        {
            var json = JsonUtility.ToJson(asset, true);

            var relativePath = AssetDatabase.GetAssetPath(asset);
            var path = $"{Application.dataPath}/{relativePath.Remove(0, 7)}";
            File.WriteAllText(path, json);
            AssetDatabase.ImportAsset(relativePath);

            base.Save();
        }

        public void CreateNewGroup()
        {
            var group = new TranslationMappingGroup("New Group");

            asset.groups.Add(group);
            tree.Reload();
            SetAssetDirty();
        }

        public void CreateNewItem()
        {
            var item = new TranslationMappingItem("New Item");

            if (asset.groups.Count == 0)
                CreateNewGroup();

            var group = tree.GetSelectedGroup() ?? asset.groups.First();
            group.items.Add(item);
            tree.Reload();
            SetAssetDirty();
        }

        public void CreateNewDynamicValue()
        {
            var val = new TranslationMappingDynamicValue("New Dynamic Value");

            if (asset.groups.Count == 0)
                CreateNewGroup();

            var group = tree.GetSelectedGroup() ?? asset.groups.First();

            if (group.items.Count == 0)
                CreateNewItem();

            var item = tree.GetSelectedItem() ?? group.items.First();
            item.dynamicValues.Add(val);
            tree.Reload();
            SetAssetDirty();
        }
    }
}