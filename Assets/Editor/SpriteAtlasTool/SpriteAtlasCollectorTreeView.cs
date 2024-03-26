
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
                base.RowGUI(args);

                Color oldColor = GUI.color;
                GUI.color = Color.gray;
                CenterRectUsingSingleLineHeight(ref cellRect);

                var iconRect = new Rect(cellRect.x + 2, cellRect.y, cellRect.height - 2, cellRect.height - 2);
                Texture2D folderIcon = EditorGUIUtility.FindTexture("Folder Icon");
                GUI.DrawTexture(iconRect, folderIcon, ScaleMode.ScaleToFit);
                var nameRect = new Rect(cellRect.x + iconRect.xMax + 2, cellRect.y, cellRect.width - iconRect.width, cellRect.height);
                if (false)
                    DefaultGUI.BoldLabel(nameRect, item.displayName, args.selected, args.focused);
                else
                    DefaultGUI.Label(nameRect, item.displayName, args.selected, args.focused);

                GUI.color = oldColor;
            }

        }

        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState()
        {
            List<MultiColumnHeaderState.Column> retVal = new List<MultiColumnHeaderState.Column>();
            MultiColumnHeaderState.Column fist = new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent("图集"),
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
            Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            if (obj != null)
            {
                Texture2D icon = AssetPreview.GetMiniThumbnail(obj);
                if (icon == null)
                    icon = AssetPreview.GetMiniTypeThumbnail(obj.GetType());
                return icon;
            }
            return null;
        }

    }

}