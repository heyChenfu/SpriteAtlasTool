using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

namespace SpriteAtlasTool 
{
    public class SpriteAtlasCollectorWindow : EditorWindow
    {
        SpriteAtlasCollectorTreeView _atlasTreeView;
        TreeViewState _treeViewState;

        [MenuItem("SpriteAtlasTool/SpriteAtlasCollectorWindow", false)]
        public static void OpenWindow()
        {
            SpriteAtlasCollectorWindow window = GetWindow<SpriteAtlasCollectorWindow>("图集收集工具", true);
            window.minSize = new Vector2(400, 400);
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            DrawToolBar();
            UpdateAssetTree();

            EditorGUILayout.EndVertical();
        }

        void DrawToolBar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("新增图集", EditorStyles.toolbarButton))
            {
                Debug.Log("新增配置");
                SpriteAtlasCollectorSetting.instance.Add();
            }

            if (GUILayout.Button("保存", EditorStyles.toolbarButton))
            {
                Debug.Log("保存");
                SpriteAtlasCollectorSetting.instance.Save();
            }

            GUILayout.EndHorizontal();

        }

        private void UpdateAssetTree()
        {
            if (SpriteAtlasCollectorSetting.instance.CollectorData == null || 
                SpriteAtlasCollectorSetting.instance.CollectorData.Count == 0)
                return;

            var root = new SpriteAtlasCollectorTreeViewItem(0, -1, null);
            root.children = new List<TreeViewItem>();
            root.displayName = "Root";
            for (int i = 0; i < SpriteAtlasCollectorSetting.instance.CollectorData.Count; ++i)
            {
                var child = new SpriteAtlasCollectorTreeViewItem(i + 1, i, SpriteAtlasCollectorSetting.instance.CollectorData[i]);
                root.AddChild(child);
            }
            if (_atlasTreeView == null)
            {
                //初始化TreeView
                if (_treeViewState == null)
                    _treeViewState = new TreeViewState();
                var headerState = SpriteAtlasCollectorTreeView.CreateDefaultMultiColumnHeaderState();
                _atlasTreeView = new SpriteAtlasCollectorTreeView(_treeViewState, headerState);
            }
            _atlasTreeView.Root = root;
            _atlasTreeView.Reload();
            _atlasTreeView.OnGUI(new Rect(0, EditorStyles.toolbar.fixedHeight, position.width, position.height - EditorStyles.toolbar.fixedHeight));
        }



    }


}

