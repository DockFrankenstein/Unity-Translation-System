using System;
using System.Collections.Generic;
using System.Linq;
using Translations.Mapping;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Translations.Editor.Mapping
{
    internal class TranslationMappingTree : TreeView
    {
        public TranslationMappingTree(TreeViewState state, TranslationMapping asset) : base(state)
        {
            Asset = asset;
            Reload();

            foreach (var item in Items)
                if (item.Value is GroupItem)
                    SetExpanded(item.Key, true);
        }

        public TranslationMapping Asset { get; set; }

        public event Action OnAssetModified;
        public event Action<object> OnDoubleClicked;
        public event Func<Item, GenericMenu> CreateContextMenu;

        private string _betterSearchString;
        public string BetterSearchString 
        {
            get => _betterSearchString;
            set
            {
                _betterSearchString = value;
                Reload();
            }
        }

        public bool ReadOnly { get; set; }

        #region Creation
        public Dictionary<int, Item> Items { get; set; } = new Dictionary<int, Item>();

        protected override TreeViewItem BuildRoot() =>
            new RootItem(Asset);

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            var list = new List<TreeViewItem>() ?? base.BuildRows(root);
            Items.Clear();
            Items.Add(0, root as Item);

            if (Asset != null)
            {
                bool isSearching = !string.IsNullOrWhiteSpace(BetterSearchString);

                foreach (var group in Asset.groups)
                {
                    var groupItem = new GroupItem(group);
                    var items = ToolGuiUtility.SortSearchList(group.items, x => x.tag, BetterSearchString);

                    if (!isSearching ||
                        items.Count() > 0)
                    {
                        root.AddChild(groupItem);
                        list.Add(groupItem);
                        Items.Add(groupItem.id, groupItem);
                    }

                    switch (IsExpanded(groupItem.id))
                    {
                        case true:
                            foreach (var item in items)
                            {
                                var itemItem = new ItemItem(item);
                                groupItem.AddChild(itemItem);
                                list.Add(itemItem);
                                Items.Add(itemItem.id, itemItem);

                                switch (IsExpanded(itemItem.id))
                                {
                                    case true:
                                        foreach (var val in item.dynamicValues)
                                        {
                                            var valItem = new DynamicValueItem(val);
                                            itemItem.AddChild(valItem);
                                            list.Add(valItem);
                                            Items.Add(valItem.id, valItem);
                                        }
                                        break;
                                    case false:
                                        if (item.dynamicValues.Count > 0)
                                            itemItem.children = CreateChildListForCollapsedParent();     
                                        break;
                                }
                            }
                            break;
                        case false:
                            if (group.items.Count > 0)
                                groupItem.children = CreateChildListForCollapsedParent();
                            break;
                    }
                }
            }

            return list;
        }
        #endregion  

        #region GUI
        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);

            if (Event.current.keyCode == KeyCode.Delete)
                DeleteSelection();
        }

        protected override float GetCustomRowHeight(int row, TreeViewItem item)
        {
            return item switch
            {
                GroupItem => 28f,
                ItemItem => 20f,
                DynamicValueItem => 16f,
                _ => base.GetCustomRowHeight(row, item),
            };
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var textRect = args.rowRect
                .BorderLeft(GetContentIndent(args.item))
                .ResizeHeightToCenter(EditorGUIUtility.singleLineHeight);

            var separatorRect = args.rowRect.ResizeToBottom(1f);

            switch (args.item)
            {
                case GroupItem group:
                    ToolGuiUtility.DrawColor(args.rowRect, args.selected ? ToolGuiUtility.SelectedColor : ToolGuiUtility.ButtonColor);
                    EditorGUI.LabelField(textRect, args.label, EditorStyles.boldLabel);
                    break;
                case ItemItem item:
                    EditorGUI.LabelField(textRect, args.label);
                    break;
                case DynamicValueItem val:
                    EditorGUI.LabelField(textRect, $"[{args.label}]");
                    break;
            }

            ToolGuiUtility.HorizontalLine(separatorRect);
            customFoldoutYOffset = (args.rowRect.height - 16f) / 2f;
        }
        #endregion

        #region Renaming
        protected override bool CanRename(TreeViewItem item) =>
            !ReadOnly;

        protected override void RenameEnded(RenameEndedArgs args)
        {
            if (!args.acceptedRename)
                return;

            var item = Items[args.itemID];
            
            switch (item)
            {
                case GroupItem groupItem:
                    groupItem.group.name = args.newName;
                    break;
                case ItemItem itemItem:
                    itemItem.item.tag = args.newName;
                    break;
                case DynamicValueItem valItem:
                    valItem.dynamicValue.tag = args.newName;
                    break;
            }

            OnAssetModified?.Invoke();
            Reload();
        }

        public void BeginRenameObject(object obj)
        {
            MakeVisible(obj);
            Reload();
            BeginRename(FindItemForObject(obj));
        }

        public void MakeVisible(object obj, bool first = true)
        {
            switch (obj)
            {
                case TranslationMappingDynamicValue val:
                    foreach (var a in Asset.groups.SelectMany(x => x.items)
                        .Where(x => x.dynamicValues.Contains(val)))
                        MakeVisible(a, false);

                    break;
                case TranslationMappingItem item:
                    foreach (var a in Asset.groups.Where(x => x.items.Contains(item)))
                        MakeVisible(a, false);

                    if (!first)
                        SetExpanded(item.GetHashCode(), true);
                    break;
                case TranslationMappingGroup group:
                    if (!first)
                        SetExpanded(group.GetHashCode(), true);
                    break;
            }
        }
        #endregion

        #region Drag & Drop
        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            if (ReadOnly)
                return false;

            var items = args.draggedItemIDs
                .Where(x => Items.ContainsKey(x))
                .Select(x => Items[x]);

            return items.All(x => x is GroupItem) ||
                items.All(x => x is ItemItem) ||
                items.All(x => x is DynamicValueItem);
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.SetGenericData("itemIDs", args.draggedItemIDs);
            DragAndDrop.SetGenericData("tree", this);
            DragAndDrop.StartDrag(Items[args.draggedItemIDs[0]].displayName);
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            if (!(DragAndDrop.GetGenericData("tree") is TranslationMappingTree sourceTree) ||
                sourceTree != this)
                return DragAndDropVisualMode.Rejected;

            IList<int> itemIDs = (IList<int>)DragAndDrop.GetGenericData("itemIDs");
            var items = itemIDs
                .Where(x => Items.ContainsKey(x))
                .Select(x => Items[x]);

            switch (items.FirstOrDefault())
            {
                case GroupItem group:
                    if (args.parentItem == rootItem)
                        break;
                    return DragAndDropVisualMode.Rejected;
                case ItemItem item:
                    if (args.parentItem is GroupItem)
                        break;
                    return DragAndDropVisualMode.Rejected;
                case DynamicValueItem val:
                    if (args.parentItem is ItemItem)
                        break;
                    return DragAndDropVisualMode.Rejected;
            }

            if (args.performDrop)
            {
                if (args.insertAtIndex == -1)
                    args.insertAtIndex = args.parentItem.children?.Count ?? 0;

                var insertIndex = args.insertAtIndex;
                foreach (var item in items)
                {
                    var index = (args.parentItem as Item).IndexOf(item);
                    if (index == -1) continue;

                    if (index < args.insertAtIndex)
                        insertIndex--;
                }

                foreach (var item in items)
                {
                    (item.parent as Item).Remove(item);
                }

                foreach (var item in items)
                {
                    (args.parentItem as Item).Insert(insertIndex, item);
                    insertIndex++;
                }

                OnAssetModified?.Invoke();
                Reload();
                SetSelection(itemIDs);
            }

            return DragAndDropVisualMode.Move;
        }
        #endregion

        public void DeleteSelection()
        {
            if (ReadOnly)
                return;

            foreach (var itemId in GetSelection())
            {
                if (!Items.ContainsKey(itemId))
                    continue;

                var item = Items[itemId];
                Delete(item.Object, true);
            }

            OnAssetModified?.Invoke();
            Reload();
        }

        public void Delete(object obj, bool silent = false)
        {
            if (ReadOnly)
                return;

            switch (obj)
            {
                case TranslationMappingGroup group:
                    Asset.groups.Remove(group);
                    break;
                case TranslationMappingItem item:
                    var groups = Asset.groups.Where(x => x.items.Contains(item));
                    foreach (var group in groups)
                        group.items.Remove(item);
                    break;
                case TranslationMappingDynamicValue dynamicValue:
                    var items = Asset.groups.SelectMany(x => x.items)
                        .Where(x => x.dynamicValues.Contains(dynamicValue));
                    foreach (var a in items)
                        a.dynamicValues.Remove(dynamicValue);
                    break;
            }

            if (!silent)
                OnAssetModified?.Invoke();
        }

        protected override void DoubleClickedItem(int id)
        {
            if (!Items.ContainsKey(id)) return;

            if (OnDoubleClicked != null)
            {
                OnDoubleClicked?.Invoke(Items[id].Object);
                return;
            }

            if (!ReadOnly)
            {
                TreeViewItem item = Items[id];
                if (CanRename(item))
                    BeginRename(item);
            }
        }

        protected override void ContextClickedItem(int id)
        {
            if (CreateContextMenu == null) return;
            if (!Items.ContainsKey(id)) return;

            var item = Items[id];
            var menu = CreateContextMenu(item);
            menu.ShowAsContext();
        }

        protected override IList<int> GetDescendantsThatHaveChildren(int id)
        {
            Stack<TreeViewItem> stack = new Stack<TreeViewItem>();

            var start = FindItem(id, rootItem);
            stack.Push(start);

            var parents = new List<int>();
            while (stack.Count > 0)
            {
                TreeViewItem item = stack.Pop();
                parents.Add(item.id);
                if (item.hasChildren)
                    for (int i = 0; i < item.children.Count; i++)
                        if (item.children[i] != null)
                            stack.Push(item.children[i]);
            }

            return parents;
        }

        #region Selection
        public TranslationMappingGroup GetSelectedGroup()
        {
            var selection = GetSelection();

            foreach (var itemId in selection)
            {
                if (!Items.ContainsKey(itemId))
                    continue;

                var item = Items[itemId];

                if (item is GroupItem groupItem)
                    return groupItem.group;
            }

            return null;
        }

        public TranslationMappingItem GetSelectedItem()
        {
            var selection = GetSelection();

            TranslationMappingItem finalItem = null;
            foreach (var itemId in selection)
            {
                if (!Items.ContainsKey(itemId))
                    continue;

                var item = Items[itemId];

                if (item is ItemItem itemItem)
                {
                    finalItem = itemItem.item;
                    break;
                }

                if (item is GroupItem groupItem)
                    finalItem ??= groupItem.group.items.FirstOrDefault();
            }

            return finalItem;
        }

        public List<object> GetSelectedObjects()
        {
            return GetSelection()
                .Where(x => Items.ContainsKey(x))
                .Select(x => Items[x].Object)
                .ToList();
        }
        #endregion

        public Item FindItemForObject(object obj) =>
            Items.TryGetValue(obj.GetHashCode(), out var val) ?
            val :
            null;

        public class Item : TreeViewItem
        {
            public virtual object Object { get; }

            public virtual void Insert(int index, Item item) { }

            public virtual void Remove(Item item) { }

            public virtual int IndexOf(Item item) => -1;
        }

        public class RootItem : Item
        {
            public RootItem(TranslationMapping asset)
            {
                this.asset = asset;
                id = 0;
                depth = -1;
            }

            public TranslationMapping asset;

            public override object Object => asset;

            public override void Insert(int index, Item item)
            {
                if (item is GroupItem group)
                    asset.groups.Insert(index, group.group);
            }

            public override void Remove(Item item)
            {
                if (item is GroupItem group)
                    asset.groups.Remove(group.group);
            }

            public override int IndexOf(Item item)
            {
                if (item is GroupItem group)
                    return asset.groups.IndexOf(group.group);

                return base.IndexOf(item);
            }
        }

        public class GroupItem : Item
        {
            public GroupItem(TranslationMappingGroup group)
            {
                this.group = group;
                id = group.GetHashCode();
                displayName = group.name;
                depth = 0;
            }

            public TranslationMappingGroup group;

            public override object Object => group;

            public override void Insert(int index, Item item)
            {
                if (item is ItemItem itemItem)
                    group.items.Insert(index, itemItem.item);
            }

            public override void Remove(Item item)
            {
                if (item is ItemItem itemItem)
                    group.items.Remove(itemItem.item);
            }

            public override int IndexOf(Item item)
            {
                if (item is ItemItem itemItem)
                    return group.items.IndexOf(itemItem.item);

                return base.IndexOf(item);
            }
        }

        public class ItemItem : Item
        {
            public ItemItem(TranslationMappingItem item)
            {
                this.item = item;
                id = item.GetHashCode();
                displayName = item.tag;
                depth = 1;
            }

            public TranslationMappingItem item;

            public override object Object => item;

            public override void Insert(int index, Item item)
            {
                if (item is DynamicValueItem val)
                    this.item.dynamicValues.Insert(index, val.dynamicValue);
            }

            public override void Remove(Item item)
            {
                if (item is DynamicValueItem val)
                    this.item.dynamicValues.Remove(val.dynamicValue);
            }

            public override int IndexOf(Item item)
            {
                if (item is DynamicValueItem val)
                    return this.item.dynamicValues.IndexOf(val.dynamicValue);

                return base.IndexOf(item);
            }
        }

        public class DynamicValueItem : Item
        {
            public DynamicValueItem(TranslationMappingDynamicValue dynamicValue)
            {
                this.dynamicValue = dynamicValue;
                id = dynamicValue.GetHashCode();
                displayName = dynamicValue.tag;
                depth = 2;
            }

            public TranslationMappingDynamicValue dynamicValue;

            public override object Object => dynamicValue;
        }
    }
}
