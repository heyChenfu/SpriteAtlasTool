using Editor.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace SpriteAtlasTool 
{
    public class SpriteAtlasCollectorWindow : EditorWindow
    {
        SpriteAtlasCollectorTreeView _atlasTreeView;
        SpriteAtlasCollectorAssetsTreeView _atlasAssetsTreeView;
        TreeViewState _atlasTreeViewState;
        TreeViewState _atlasAssetsTreeViewState;
        public SpriteAtlasCollectorData SelectCollectorData { get; private set; }

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
            Rect[] horSplitRect = GUIRectCalculator.Split(contentRect, splitArr, true);
            float[] splitArr2 = new float[] { 0.6f, 0.4f };
            Rect[] verSlitRect = GUIRectCalculator.Split(horSplitRect[1], splitArr2, false);

            DrawToolBar();
            DrawAtlasCollectorTree(horSplitRect[0]);
            DrawCollectorInfo(verSlitRect[0]);
            DrawCollectorSingleDataTree(verSlitRect[1]);

            EditorGUILayout.EndVertical();
        }

        void DrawToolBar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label(SpriteAtlasToolLanguageDef.Outputpath, GUILayout.MaxWidth(80));
            //SpriteAtlasOutputPath
            string newPath = EditorGUILayout.TextField(SpriteAtlasCollectorSetting.instance.SpriteAtlasOutputPath);
            newPath?.TrimEnd('/', '\\');
            if (SpriteAtlasCollectorSetting.instance.SpriteAtlasOutputPath != newPath)
            {
                if (Directory.Exists(newPath))
                    SpriteAtlasCollectorSetting.instance.SpriteAtlasOutputPath = newPath;
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(SpriteAtlasToolLanguageDef.BuildSpriteAtlas, EditorStyles.toolbarButton))
            {
                Debug.Log(SpriteAtlasToolLanguageDef.BuildSpriteAtlas);
                if (!Directory.Exists(SpriteAtlasCollectorSetting.instance.SpriteAtlasOutputPath))
                    EditorUtility.DisplayDialog(SpriteAtlasToolLanguageDef.ErrorMsg, SpriteAtlasToolLanguageDef.ErrorPath, "ok");
                else
                    SetSpriteAtlas();
            }
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

        private void DrawAtlasCollectorTree(Rect treeviewRect)
        {
            if (SpriteAtlasCollectorSetting.instance.CollectorData == null || 
                SpriteAtlasCollectorSetting.instance.CollectorData.Count == 0)
                return;

            if (_atlasTreeView == null)
            {
                //初始化TreeView
                if (_atlasTreeViewState == null)
                    _atlasTreeViewState = new TreeViewState();
                var headerState = SpriteAtlasCollectorTreeView.CreateDefaultMultiColumnHeaderState();
                _atlasTreeView = new SpriteAtlasCollectorTreeView(_atlasTreeViewState, headerState, this);
            }
            _atlasTreeView.Reload();
            _atlasTreeView.OnGUI(treeviewRect);

        }

        private void DrawCollectorInfo(Rect rect)
        {
            GUILayout.BeginArea(rect);

            if (SelectCollectorData != null)
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                GUILayout.Label(SpriteAtlasToolLanguageDef.CollectList);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(40)))
                {
                    if (SelectCollectorData != null)
                        SelectCollectorData.SpriteAtlasData.Add(new SpriteAtlasCollectorSingleData());
                }
                GUILayout.EndHorizontal();

                for (int i = 0; i < SelectCollectorData.SpriteAtlasData.Count; ++i)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(string.Format(SpriteAtlasToolLanguageDef.CollectIndex, i + 1), GUILayout.Width(100));
                    var newObj = EditorGUILayout.ObjectField("", SelectCollectorData.SpriteAtlasData[i].PathData, typeof(UnityEngine.Object), false);
                    if (newObj != SelectCollectorData.SpriteAtlasData[i].PathData)
                    {
                        SelectCollectorData.SpriteAtlasData[i].PathData = newObj;

                    }
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        SelectCollectorData.SpriteAtlasData.RemoveAt(i);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndArea();
        }

        private void DrawCollectorSingleDataTree(Rect treeviewRect)
        {
            if (SelectCollectorData == null)
                return;

            if (_atlasAssetsTreeView == null)
            {
                //初始化TreeView
                if (_atlasAssetsTreeViewState == null)
                    _atlasAssetsTreeViewState = new TreeViewState();
                var headerState = SpriteAtlasCollectorAssetsTreeView.CreateDefaultMultiColumnHeaderState();
                _atlasAssetsTreeView = new SpriteAtlasCollectorAssetsTreeView(_atlasAssetsTreeViewState, headerState, this);
            }
            _atlasAssetsTreeView.Reload();
            _atlasAssetsTreeView.OnGUI(treeviewRect);
        }

        public void SpriteAtlasCollectorDataSelect(SpriteAtlasCollectorData data)
        {
            SelectCollectorData = data;

        }

        #region 打图集

        public static void SetSpriteAtlas()
        {
            EditorUtility.DisplayCancelableProgressBar(SpriteAtlasToolLanguageDef.BeginSetting, "", 1);

            try
            {
                for (int i = 0; SpriteAtlasCollectorSetting.instance.CollectorData != null &&
                i < SpriteAtlasCollectorSetting.instance.CollectorData.Count; i++)
                {
                    //生成/获取SpriteAtlas
                    if (!Directory.Exists(SpriteAtlasCollectorSetting.instance.SpriteAtlasOutputPath))
                        break;
                    string sAtlasPath = Path.Combine(SpriteAtlasCollectorSetting.instance.SpriteAtlasOutputPath,
                        $"{SpriteAtlasCollectorSetting.instance.CollectorData[i].Name}.spriteatlas");
                    SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(sAtlasPath);
                    if (atlas == null)
                        atlas = CreateSpriteAtlas(sAtlasPath);
                    UnityEngine.Object[] packables = atlas.GetPackables();
                    List<UnityEngine.Object> newPackableList = new List<UnityEngine.Object>();

                    for (int j = 0; j < SpriteAtlasCollectorSetting.instance.CollectorData[i].SpriteAtlasData.Count; ++j)
                    {
                        UnityEngine.Object objData = SpriteAtlasCollectorSetting.instance.CollectorData[i].SpriteAtlasData[j].PathData;
                        string objPath = AssetDatabase.GetAssetPath(objData);
                        bool isFolder = AssetDatabase.IsValidFolder(objPath);
                        if (isFolder)
                        {
                            string[] filePath = System.IO.Directory.GetFiles(objPath);
                            if (filePath.Length <= 0)
                                continue;
                            //获取目录下所有图片
                            for (int z = 0; z < filePath.Length; z++)
                            {
                                filePath[z] = filePath[z].Replace("\\", "/");
                                AddNewSpriteToAtlas(filePath[z], packables, newPackableList);
                            }
                        }
                        else
                        {
                            //非文件夹直接添加
                            AddNewSpriteToAtlas(objPath, packables, newPackableList);
                        }
                    }
                    atlas.Add(newPackableList.ToArray());
                }

            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                SpriteAtlasUtility.PackAllAtlases(EditorUserBuildSettings.activeBuildTarget, false);
                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();

            }

        }

        /// <summary>
        /// 创建图集, 对图集进行统一设置
        /// </summary>
        /// <param name="atlasPath"></param>
        /// <returns></returns>
        private static SpriteAtlas CreateSpriteAtlas(string atlasPath)
        {
            SpriteAtlas atlas = new SpriteAtlas();
            AssetDatabase.CreateAsset(atlas, atlasPath);
            atlas.SetIncludeInBuild(false);
            SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings()
            {
                blockOffset = 1,
                enableRotation = false,
                enableTightPacking = false,
                padding = 4,
            };
            atlas.SetPackingSettings(packSetting);
            SpriteAtlasTextureSettings textureSetting = new SpriteAtlasTextureSettings()
            {
                readable = false,
                generateMipMaps = false,
                sRGB = true,
                filterMode = FilterMode.Bilinear,
            };
            atlas.SetTextureSettings(textureSetting);

            int maxSize = 2048; //最大尺寸
            int qualityLevel = 100;
            TextureImporterPlatformSettings atlasAndSetting = atlas.GetPlatformSettings("Android");
            if (atlasAndSetting == null)
                atlasAndSetting = new TextureImporterPlatformSettings();
            atlasAndSetting.overridden = true;
            atlasAndSetting.name = "Android";
            atlasAndSetting.maxTextureSize = maxSize;
            atlasAndSetting.compressionQuality = qualityLevel;
            atlasAndSetting.format = TextureImporterFormat.ASTC_4x4;
            atlas.SetPlatformSettings(atlasAndSetting);

            TextureImporterPlatformSettings atlasiOSSetting = atlas.GetPlatformSettings("iPhone");
            if (atlasiOSSetting == null)
                atlasiOSSetting = new TextureImporterPlatformSettings();
            atlasiOSSetting.overridden = true;
            atlasAndSetting.name = "iPhone";
            atlasiOSSetting.maxTextureSize = maxSize;
            atlasiOSSetting.compressionQuality = qualityLevel;
            atlasiOSSetting.format = TextureImporterFormat.ASTC_4x4;
            atlas.SetPlatformSettings(atlasiOSSetting);

            return atlas;
        }

        private static void AddNewSpriteToAtlas(string filePath, UnityEngine.Object[] packables, List<UnityEngine.Object> newPackableList)
        {
            UnityEngine.Object spriteObj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(filePath);
            if (!(spriteObj is Texture2D))
                return;//过滤其他文件
            if (Array.Find(packables, x => x == spriteObj) == null)
                newPackableList.Add(spriteObj);
        }

        #endregion

    }

}

