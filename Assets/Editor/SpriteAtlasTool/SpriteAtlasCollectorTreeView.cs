using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace SpriteAtlasTool
{
    public class SpriteAtlasCollectorTreeViewItem : TreeViewItem
    {
        public SpriteAtlasCollectorData Data;

        public SpriteAtlasCollectorTreeViewItem(int id, int depth, SpriteAtlasCollectorData data) : base(id, depth)
        {
            Data = data;
        }
    }

    public class SpriteAtlasCollectorTreeView : TreeView 
    {
        public MultiColumnHeaderState HeaderState;

        private SpriteAtlasCollectorTreeViewItem _root;
        private SpriteAtlasCollectorWindow _mainWindow;

        public SpriteAtlasCollectorTreeView(TreeViewState state, MultiColumnHeaderState header, SpriteAtlasCollectorWindow mainWindow)
            : base(state, new MultiColumnHeader(header))
        {
            showBorder = true;
            showAlternatingRowBackgrounds = false;
            HeaderState = header;
            _mainWindow = mainWindow;
        }

        protected override TreeViewItem BuildRoot()
        {
            _root = new SpriteAtlasCollectorTreeViewItem(0, -1, null);
            _root.children = new List<TreeViewItem>();
            _root.displayName = "Root";
            for (int i = 0; i < SpriteAtlasCollectorSetting.instance.CollectorData.Count; ++i)
            {
                var child = new SpriteAtlasCollectorTreeViewItem(i + 1, i, SpriteAtlasCollectorSetting.instance.CollectorData[i]);
                if (string.IsNullOrEmpty(child.Data.Name))
                    child.Data.Name = $"{SpriteAtlasToolLanguageDef.SpriteAtlas}{i + 1}";
                child.displayName = child.Data.Name;
                _root.AddChild(child);
            }
            return _root;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                CellGUI(args.GetCellRect(i), args.item, ref args);
            }
        }

        /// <summary>
        /// 绘制列表中的每项内容
        /// </summary>
        /// <param name="cellRect"></param>
        /// <param name="item"></param>
        /// <param name="args"></param>
        void CellGUI(Rect cellRect, TreeViewItem item, ref RowGUIArgs args)
        {
            if (item is SpriteAtlasCollectorTreeViewItem treeViewItem)
            {
                CenterRectUsingSingleLineHeight(ref cellRect);

                var iconRect = new Rect(cellRect.x + 2, cellRect.y, cellRect.height - 2, cellRect.height - 2);
                Texture2D folderIcon = EditorGUIUtility.FindTexture("Folder Icon");
                GUI.DrawTexture(iconRect, folderIcon, ScaleMode.ScaleToFit);
                var nameRect = new Rect(cellRect.x + iconRect.xMax + 2, cellRect.y, cellRect.width - iconRect.width, cellRect.height);
                DefaultGUI.Label(nameRect, item.displayName, args.selected, args.focused);
            }

        }

        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState()
        {
            List<MultiColumnHeaderState.Column> retVal = new List<MultiColumnHeaderState.Column>();
            MultiColumnHeaderState.Column fist = new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent(SpriteAtlasToolLanguageDef.SpriteAtlas),
                minWidth = 60,
                width = 300,
                sortedAscending = true,
                headerTextAlignment = TextAlignment.Center,
                canSort = false,
                autoResize = true,
                allowToggleVisibility = false
            };
            retVal.Add(fist);
            return new MultiColumnHeaderState(retVal.ToArray());
        }

        private void CreateTreeViewItem()
        {
            SpriteAtlasCollectorSetting.instance.Add();
        }

        private void RenameTreeViewItem(object userData)
        {
            if(userData is IList<int> selectList)
            {
                for (int i = 0; i < selectList.Count; ++i)
                {
                    TreeViewItem item = FindItem(selectList[i], rootItem);
                    if(item != null)
                        BeginRename(item);
                }
            }

        }

        private void DeleteTreeViewItem(object userData)
        {
            if (userData is IList<int> selectList)
            {
                for (int i = 0; i < selectList.Count; ++i)
                {
                    TreeViewItem item = FindItem(selectList[i], rootItem);
                    if (item != null && item is SpriteAtlasCollectorTreeViewItem collectorItem)
                        SpriteAtlasCollectorSetting.instance.Remove(collectorItem.Data);
                }
            }
        }

        #region 点击

        /// <summary>
        /// 右键空白处
        /// </summary>
        protected override void ContextClicked()
        {
            if (HasSelection())
            {
                // 清除当前的选择
                SetSelection(new int[] { });
                _mainWindow.SpriteAtlasCollectorDataSelect(null);
                return;
            }

            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent(SpriteAtlasToolLanguageDef.AddNewSpriteAtlas), false, CreateTreeViewItem);
            menu.ShowAsContext();
        }

        /// <summary>
        /// 点击item
        /// </summary>
        /// <param name="id"></param>
        protected override void ContextClickedItem(int id)
        {
            IList<int> selectList = GetSelection();
            if (selectList == null || selectList.Count == 0)
                return;

            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent(SpriteAtlasToolLanguageDef.Rename), false, RenameTreeViewItem, selectList);
            menu.AddItem(new GUIContent(SpriteAtlasToolLanguageDef.Delete), false, DeleteTreeViewItem, selectList);
            menu.ShowAsContext();

        }

        #endregion

        /// <summary>
        /// 选择变化回调
        /// </summary>
        /// <param name="selectedIds"></param>
        protected override void SelectionChanged(IList<int> selectedIds)
        {
            SpriteAtlasCollectorData selectData = null;
            foreach (var id in selectedIds)
            {
                TreeViewItem item = FindItem(id, rootItem);
                if (item != null && item is SpriteAtlasCollectorTreeViewItem collectorItem)
                {
                    selectData = collectorItem.Data;
                    break;
                }
            }
            _mainWindow.SpriteAtlasCollectorDataSelect(selectData);

        }

        /// <summary>
        /// 改名判断回调
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool CanRename(TreeViewItem item)
        {
            return item.displayName.Length > 0;
        }

        /// <summary>
        /// 重命名回调
        /// </summary>
        /// <param name="args"></param>
        protected override void RenameEnded(RenameEndedArgs args)
        {
            if (!string.IsNullOrEmpty(args.newName) && args.newName != args.originalName)
            {
                for (int i = 0; i < rootItem.children.Count; ++i)
                {
                    if (rootItem.children[i].id == args.itemID)
                    {
                        if (rootItem.children[i] is SpriteAtlasCollectorTreeViewItem collectorItem)
                        {
                            collectorItem.Data.Name = args.newName;
                            break;
                        }
                    }
                }
                Reload();
            }
            else
            {
                args.acceptedRename = false;
            }
        }

    }

}