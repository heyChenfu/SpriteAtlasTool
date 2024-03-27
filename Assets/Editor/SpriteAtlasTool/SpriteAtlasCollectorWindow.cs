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
            SpriteAtlasCollectorWindow window = GetWindow<SpriteAtlasCollectorWindow>(SpriteAtlasToolLanguageDef.MainWindowName, true);
            window.minSize = new Vector2(400, 400);
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            float barHeight = EditorStyles.toolbar.fixedHeight;
            Rect contentRect = new Rect(0, barHeight, position.width, position.height - barHeight);
            float[] splitArr = new float[] { 0.4f, 0.6f };
            Rect[] splitRect = GUIRectCalculator.Split(contentRect, splitArr, true);

            DrawToolBar();
            UpdateAssetTree(splitRect[0]);

            EditorGUILayout.EndVertical();
        }

        void DrawToolBar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(SpriteAtlasToolLanguageDef.AddNewSpriteAtlas, EditorStyles.toolbarButton))
            {
                Debug.Log(SpriteAtlasToolLanguageDef.AddNewSpriteAtlas);
                SpriteAtlasCollectorSetting.instance.Add();
            }

            if (GUILayout.Button(SpriteAtlasToolLanguageDef.SaveData, EditorStyles.toolbarButton))
            {
                Debug.Log(SpriteAtlasToolLanguageDef.SaveData);
                SpriteAtlasCollectorSetting.instance.Save();
            }

            GUILayout.EndHorizontal();

        }

        private void UpdateAssetTree(Rect treeviewRect)
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
                child.displayName = $"{SpriteAtlasToolLanguageDef.SpriteAtlas}{i + 1}";
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
            _atlasTreeView.OnGUI(treeviewRect);

        }

    }

}

