using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpriteAtlasTool
{
    [FilePath("Assets/SpriteAtlasCollectorSetting.asset", FilePathAttribute.Location.ProjectFolder)]
    [CreateAssetMenu(fileName = "SpriteAtlasCollectorSetting", menuName = "SpriteAtlasTool/Create SpriteAtlasCollectorSetting")]
    public class SpriteAtlasCollectorSetting : ScriptableSingleton<SpriteAtlasCollectorSetting>
    {
        public List<SpriteAtlasCollectorData> CollectorData;

        public void Add()
        {
            if (CollectorData == null)
                CollectorData = new List<SpriteAtlasCollectorData>();
            CollectorData.Add(new SpriteAtlasCollectorData());
        }

        public void Remove(SpriteAtlasCollectorData data)
        {
            CollectorData.Remove(data);
        }

        public void Save()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssets();
#endif
        }

    }

}

