﻿using System;
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
        }

        public TranslationMapping Asset { get; set; }

        public event Action OnAssetModified;
        public event Action<object> OnDoubleClicked;

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

        #region Creation
        Dictionary<int, Item> Items { get; set; } = new Dictionary<int, Item>();

        protected override TreeViewItem BuildRoot() =>
            new TreeViewItem(-1, -1);

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            var list = new List<TreeViewItem>() ?? base.BuildRows(root);
            Items.Clear();

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

                    if (IsExpanded(groupItem.id))
                    {
                        foreach (var item in items)
                        {
                            var itemItem = new ItemItem(item);
                            groupItem.AddChild(itemItem);
                            list.Add(itemItem);
                            Items.Add(itemItem.id, itemItem);

                            if (IsExpanded(itemItem.id))
                            {
                                foreach (var val in item.dynamicValues)
                                {
                                    var valItem = new DynamicValueItem(val);
                                    itemItem.AddChild(valItem);
                                    list.Add(valItem);
                                    Items.Add(valItem.id, valItem);
                                }
                            }

                            if (item.dynamicValues.Count > 0)
                                itemItem.children = CreateChildListForCollapsedParent();
                        }
                    }

                    if (group.items.Count > 0)
                        groupItem.children = CreateChildListForCollapsedParent();
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

            var outlineRect = args.rowRect.ResizeToBottom(0f);

            EditorGUI.LabelField(textRect, args.label);
            ToolGuiUtility.HorizontalLine(outlineRect);

            customFoldoutYOffset = (args.rowRect.height - 16f) / 2f;
        }
        #endregion

        #region Renaming
        protected override bool CanRename(TreeViewItem item) =>
            true;

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
        #endregion

        public void DeleteSelection()
        {
            foreach (var itemId in GetSelection())
            {
                if (!Items.ContainsKey(itemId))
                    continue;

                var item = Items[itemId];
                
                switch (item)
                {
                    case GroupItem groupItem:
                        Asset.groups.Remove(groupItem.group);
                        break;
                    case ItemItem itemItem:
                        var groups = Asset.groups.Where(x => x.items.Contains(itemItem.item));
                        foreach (var group in groups)
                            group.items.Remove(itemItem.item);
                        break;
                    case DynamicValueItem dynamicValue:
                        var items = Asset.groups.SelectMany(x => x.items)
                            .Where(x => x.dynamicValues.Contains(dynamicValue.dynamicValue));
                        foreach (var a in items)
                            a.dynamicValues.Remove(dynamicValue.dynamicValue);
                        break;
                }
            }

            Reload();
        }

        protected override void DoubleClickedItem(int id)
        {
            if (!Items.ContainsKey(id)) return;

            if (OnDoubleClicked != null)
            {
                OnDoubleClicked?.Invoke(Items[id].Object);
                return;
            }

            TreeViewItem item = Items[id];
            if (CanRename(item))
                BeginRename(item);
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

        class Item : TreeViewItem
        {
            public virtual object Object { get; }
        }

        class GroupItem : Item
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
        }

        class ItemItem : Item
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
        }

        class DynamicValueItem : Item
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