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
        public SpriteAtlasCollectorTreeViewItem Root;
        public MultiColumnHeaderState HeaderState;

        public SpriteAtlasCollectorTreeView(TreeViewState state, MultiColumnHeaderState header)
            : base(state, new MultiColumnHeader(header))
        {
            showBorder = true;
            showAlternatingRowBackgrounds = false;
            HeaderState = header;
        }

        protected override TreeViewItem BuildRoot()
        {
            return Root;
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
                string sName = string.IsNullOrEmpty(treeViewItem.Data.Name) ? item.displayName : treeViewItem.Data.Name;
                DefaultGUI.Label(nameRect, sName, args.selected, args.focused);
            }

        }

        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState()
        {
            List<MultiColumnHeaderState.Column> retVal = new List<MultiColumnHeaderState.Column>();
            MultiColumnHeaderState.Column fist = new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent(SpriteAtlasToolLanguageDef.SpriteAtlas),
                minWidth = 60,
                width = 200,
                sortedAscending = true,
                headerTextAlignment = TextAlignment.Center,
                canSort = false,
                autoResize = true,
                allowToggleVisibility = false
            };
            retVal.Add(fist);
            return new MultiColumnHeaderState(retVal.ToArray());
        }

        private Texture2D GetIcon(string path)
        {
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
            if (obj != null)
            {
                Texture2D icon = AssetPreview.GetMiniThumbnail(obj);
                if (icon == null)
                    icon = AssetPreview.GetMiniTypeThumbnail(obj.GetType());
                return icon;
            }
            return null;
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