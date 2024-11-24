using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using Translations.Mapping;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Translations.Editor.Mapping
{
    internal class MappingWindow : AssetEditorWindow<MappingWindow, Translations.Mapping.Mapping>
    {
        public override string WindowTitle => "Translation Mapping Editor";

        public override string PrefsKeyPrefix => "transmap";

        public MappingEditorToolbar toolbar;
        public MappingTree tree;
        public TreeViewState treeState;
        public MappingWindowInspector inspector;

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            object obj = EditorUtility.InstanceIDToObject(instanceID);
            if (!(obj is Translations.Mapping.Mapping asset))
                return false;

            OpenAsset(asset);
            return true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            toolbar = new MappingEditorToolbar(this);

            if (treeState == null)
                treeState = new TreeViewState();

            tree = new MappingTree(treeState, asset);
            tree.OnAssetModified += SetAssetDirty;
            tree.CreateContextMenu += item =>
            {
                var menu = new GenericMenu();

                if (item is MappingTree.GroupItem group)
                {
                    menu.AddItem("Add Item", false, () => CreateNewItem(group.group));
                    menu.AddSeparator("");
                }

                if (item is MappingTree.ItemItem itemItem)
                {
                    menu.AddItem("Add Dynamic Value", false, () => CreateNewDynamicValue(itemItem.item));
                    menu.AddSeparator("");
                }

                menu.AddItem("Rename", false, () => tree.BeginRenameObject(item.Object));
                menu.AddItem("Delete", false, () => tree.Delete(item.Object));

                return menu;
            };

            if (inspector == null)
                inspector = new MappingWindowInspector(this);

            inspector.Initialize();
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            toolbar.OnGUI();

            using (new GUILayout.HorizontalScope())
            {
                DrawTreeView(tree);
                using (new GUILayout.VerticalScope(GUILayout.Width(inspector.Width)))
                {
                    inspector.OnGUI();
                    GUILayout.FlexibleSpace();
                }

                ToolGuiUtility.VerticalLine(GUILayoutUtility.GetLastRect().ResizeToLeft(0f));
            }
        }

        public override void SetAssetDirty()
        {
            base.SetAssetDirty();
            tree.Reload();
        }

        public void CreateNewGroup()
        {
            var group = new MappingGroup("New Group");

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

        public void CreateNewItem(MappingGroup group)
        {
            var item = new MappingItem("New Item");

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

        public void CreateNewDynamicValue(MappingItem item)
        {
            var val = new MappingDynamicValue("New Dynamic Value");
            item.dynamicValues.Add(val);
            tree.Reload();
            tree.BeginRenameObject(val);
            SetAssetDirty();
        }
    }
}