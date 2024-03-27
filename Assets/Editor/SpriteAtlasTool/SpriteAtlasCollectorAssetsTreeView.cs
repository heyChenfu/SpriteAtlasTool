
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.VersionControl;
using UnityEngine;

namespace SpriteAtlasTool
{
    public sealed class AssetTreeViewItem : TreeViewItem
    {
        private UnityEngine.Object _asset;

        public UnityEngine.Object TargetAsset => _asset;

        public AssetTreeViewItem(int id, string name, UnityEngine.Object asset) : base(id, id, name)
        {
            _asset = asset;
        }
    }

    public class SpriteAtlasCollectorAssetsTreeView : TreeView
    {
        private TreeViewItem _root;
        private SpriteAtlasCollectorWindow _mainWindow;

        public SpriteAtlasCollectorAssetsTreeView(TreeViewState state, MultiColumnHeaderState header, SpriteAtlasCollectorWindow mainWindow)
            : base(state, new MultiColumnHeader(header))
        {
            showBorder = true;
            showAlternatingRowBackgrounds = true;
            _mainWindow = mainWindow;
        }

        protected override TreeViewItem BuildRoot()
        {
            _root = new TreeViewItem{ id = -1, depth = -1, children = new List<TreeViewItem>() };
            if (_mainWindow.SelectCollectorData != null)
            {
                int countId = 1;
                for (int i = 0; i < _mainWindow.SelectCollectorData.SpriteAtlasData.Count; ++i)
                {
                    UnityEngine.Object objData = _mainWindow.SelectCollectorData.SpriteAtlasData[i].PathData;
                    string objPath = AssetDatabase.GetAssetPath(objData);
                    bool isFolder = AssetDatabase.IsValidFolder(objPath);
                    if (isFolder)
                    {
                        string[] allFolderAssets = AssetDatabase.FindAssets("", new[] { objPath });
                        for (int j = 0; j < allFolderAssets.Length; ++j)
                        {
                            string subObjPath = AssetDatabase.GUIDToAssetPath(allFolderAssets[j]);
                            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(subObjPath);
                            if (obj != null)
                            {
                                var t = new AssetTreeViewItem(countId, subObjPath, obj);
                                _root.AddChild(t);
                                countId++;
                            }
                        }
                    }
                    else
                    {
                        var t = new AssetTreeViewItem(countId, objPath, objData);
                        _root.AddChild(t);
                        countId++;
                    }
                }
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
            if (item is AssetTreeViewItem treeViewItem)
            {
                CenterRectUsingSingleLineHeight(ref cellRect);

                var iconRect = new Rect(cellRect.x + 2, cellRect.y, cellRect.height - 2, cellRect.height - 2);
                Texture2D folderIcon = GetIcon(treeViewItem.TargetAsset);
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
                headerContent = new GUIContent(SpriteAtlasToolLanguageDef.FilePath),
                minWidth = 60,
                width = 400,
                sortedAscending = true,
                headerTextAlignment = TextAlignment.Center,
                canSort = false,
                autoResize = true,
                allowToggleVisibility = false
            };
            retVal.Add(fist);
            return new MultiColumnHeaderState(retVal.ToArray());
        }

        protected override void SingleClickedItem(int id)
        {
            TreeViewItem item = FindItem(id, _root);
            if (item != null && item is AssetTreeViewItem treeViewItem)
            {
                //或者使用EditorUtility.RevealInFinder
                EditorGUIUtility.PingObject(treeViewItem.TargetAsset);
            }

        }

        private Texture2D GetIcon(string path)
        {
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
            return GetIcon(obj);
        }

        private Texture2D GetIcon(UnityEngine.Object obj)
        {
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