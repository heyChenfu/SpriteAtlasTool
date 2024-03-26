
using System.Collections.Generic;
using System;

namespace SpriteAtlasTool
{

    [Serializable]
    public class SpriteAtlasCollectorData
    {
        public List<SpriteAtlasCollectorSingleData> SpriteAtlasData = new List<SpriteAtlasCollectorSingleData>();
    }

    [Serializable]
    public class SpriteAtlasCollectorSingleData
    {
        /// <summary>
        /// 收集路径
        /// 支持文件夹或单个资源文件
        /// </summary>
        public UnityEngine.Object PathData = null;

    }

}