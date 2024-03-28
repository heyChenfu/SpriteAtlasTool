

using UnityEngine;

namespace Editor.Common
{

    public static class GUIRectCalculator
    {
        
        /// <summary>
        /// 分割矩形
        /// </summary>
        /// <param name="content"></param>
        /// <param name="splitRatio"></param>
        /// <param name="bHorizental"></param>
        /// <returns></returns>
        public static Rect[] Split(Rect content, float[] splitRatio, bool bHorizental)
        {
            Rect[] rectArr = new Rect[splitRatio.Length];
            float xOffset = content.x;
            float yOffset = content.y;
            for (int i = 0; i < splitRatio.Length; ++i)
            {
                if (bHorizental)
                {
                    float newRectWidth = content.width * splitRatio[i];
                    rectArr[i] = new Rect(xOffset, yOffset, newRectWidth, content.height);
                    xOffset += newRectWidth;
                }
                else
                {
                    float newRectHeight = content.height * splitRatio[i];
                    rectArr[i] = new Rect(xOffset, yOffset, content.width, newRectHeight);
                    yOffset += newRectHeight;
                }
            }
            return rectArr;
        }

    }

}
