using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using Translations.Mapping;
using UnityEditor.Callbacks;

namespace Translations.Editor.Mapping
{
    internal class TranslationMappingWindow : AssetEditorWindow<TranslationMappingWindow, TranslationMapping>
    {
        public override string WindowTitle => "Translation Mapping Editor";

        public override string PrefsKeyPrefix => "transmap";

        public TranslationMappingEditorToolbar toolbar;
        public TranslationMappingTree tree;
        public TreeViewState treeState;

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            object obj = EditorUtility.InstanceIDToObject(instanceID);
            if (!(obj is TranslationMapping asset))
                return false;

            OpenAsset(asset);
            return true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            toolbar = new TranslationMappingEditorToolbar(this);

            if (treeState == null)
                treeState = new TreeViewState();

            tree = new TranslationMappingTree(treeState, asset);
            tree.OnAssetModified += SetAssetDirty;
            tree.CreateContextMenu += item =>
            {
                var menu = new GenericMenu();

                if (item is TranslationMappingTree.GroupItem group)
                {
                    menu.AddItem("Add Item", false, () => CreateNewItem(group.group));
                    menu.AddSeparator("");
                }

                if (item is TranslationMappingTree.ItemItem itemItem)
                {
                    menu.AddItem("Add Dynamic Value", false, () => CreateNewDynamicValue(itemItem.item));
                    menu.AddSeparator("");
                }

                menu.AddItem("Rename", false, () => tree.BeginRenameObject(item.Object));
                menu.AddItem("Delete", false, () => tree.Delete(item.Object));

                return menu;
            };
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            toolbar.OnGUI();
            DrawTreeView(tree);
        }

        public override void SetAssetDirty()
        {
            base.SetAssetDirty();
            tree.Reload();
        }

        public void CreateNewGroup()
        {
            var group = new TranslationMappingGroup("New Group");

            asset.groups.Add(group);
            tree.Reload();
            tree.BeginRenameObject(group);
            SetAssetDirty();
        }

        public void CreateNewItem()
        {
            if (asset.groups.Count == 0)
                CreateNewGroup();

            var group = tree.GetSelectedGroup() ?? asset.groups.First();
            CreateNewItem(group);
        }

        public void CreateNewItem(TranslationMappingGroup group)
        {
            var item = new TranslationMappingItem("New Item");

            group.items.Add(item);
            tree.Reload();
            tree.BeginRenameObject(item);
            SetAssetDirty();
        }

        public void CreateNewDynamicValue()
        {
            if (asset.groups.Count == 0)
                CreateNewGroup();

            var group = tree.GetSelectedGroup() ?? asset.groups.First();

            if (group.items.Count == 0)
                CreateNewItem();

            var item = tree.GetSelectedItem() ?? group.items.First();
            CreateNewDynamicValue(item);
        }

        public void CreateNewDynamicValue(TranslationMappingItem item)
        {
            var val = new TranslationMappingDynamicValue("New Dynamic Value");
            item.dynamicValues.Add(val);
            tree.Reload();
            tree.BeginRenameObject(val);
            SetAssetDirty();
        }
    }
}