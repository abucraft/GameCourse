using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace MemoryTrap
{
    public class RoomPatternScale 
    {
        static Color32[] keyColors = { Wall.editColor,WallCorner.editColor,Door.editColor};

        public static void Scale(Texture2D tex, int newWidth,int newHeight)
        {
            int oldWidth = tex.width;
            int oldHeight = tex.height;
            Color32[] oldColors = tex.GetPixels32();
            float scaleX = newWidth / (float)oldWidth;
            float scaleY = newHeight / (float)oldHeight;
            Color32[] newColors = new Color32[newHeight * newWidth];
            for(int i = 0; i < newWidth * newHeight; i++)
            {
                newColors[i] = new Color32(0,0,0,0);
            }
            for(int y = 0; y < oldHeight; y++)
            {
                for(int x = 0; x < oldWidth; x++)
                {
                    if (ColorIdx(oldColors[y * oldWidth + x])> ColorIdx(newColors[((int)(scaleY * y)) * newWidth + (int)(scaleX * x)]))
                    {
                        //Debug.Log("key color at (" + x.ToString() + ',' + y.ToString() + ')');
                        
                        newColors[((int)(scaleY * y))*newWidth + (int)(scaleX * x)] = oldColors[y * oldWidth + x];
                    }
                }
            }
            for(int y = 0; y < newHeight; y++)
            {
                for(int x = 0; x < newWidth; x++)
                {
                    
                    if (newColors[y * newWidth + x].a==0)
                    {
                        newColors[y * newWidth + x] = oldColors[((int)(y / scaleY)) * oldWidth + (int)(x / scaleX)];
                    }
                    //Debug.Log("color at (" + x.ToString() + ',' + y.ToString() + "):" + newColors[y * newWidth + x].ToString());
                }
            }
            tex.Resize(newWidth, newHeight);
            tex.SetPixels32(newColors);
            tex.Apply();
        }

        public static Color32[] ScaleColor(Texture2D tex, int newWidth, int newHeight)
        {
            int oldWidth = tex.width;
            int oldHeight = tex.height;
            Color32[] oldColors = tex.GetPixels32();
            float scaleX = newWidth / (float)oldWidth;
            float scaleY = newHeight / (float)oldHeight;
            Color32[] newColors = new Color32[newHeight * newWidth];
            for (int i = 0; i < newWidth * newHeight; i++)
            {
                newColors[i] = new Color32(0, 0, 0, 0);
            }
            for (int y = 0; y < oldHeight; y++)
            {
                for (int x = 0; x < oldWidth; x++)
                {
                    if (ColorIdx(oldColors[y * oldWidth + x]) > ColorIdx(newColors[((int)(scaleY * y)) * newWidth + (int)(scaleX * x)]))
                    {
                        //Debug.Log("key color at (" + x.ToString() + ',' + y.ToString() + ')');

                        newColors[((int)(scaleY * y)) * newWidth + (int)(scaleX * x)] = oldColors[y * oldWidth + x];
                    }
                }
            }
            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {

                    if (newColors[y * newWidth + x].a == 0)
                    {
                        newColors[y * newWidth + x] = oldColors[((int)(y / scaleY)) * oldWidth + (int)(x / scaleX)];
                    }
                    //Debug.Log("color at (" + x.ToString() + ',' + y.ToString() + "):" + newColors[y * newWidth + x].ToString());
                }
            }
            return newColors;
        }

        public static Color32[] ScaleColor(RoomPattern pattern, int newWidth, int newHeight)
        {
            int oldWidth = pattern.width;
            int oldHeight = pattern.height;
            Color32[] oldColors = pattern.color;
            float scaleX = newWidth / (float)oldWidth;
            float scaleY = newHeight / (float)oldHeight;
            Color32[] newColors = new Color32[newHeight * newWidth];
            for (int i = 0; i < newWidth * newHeight; i++)
            {
                newColors[i] = new Color32(0, 0, 0, 0);
            }
            for (int y = 0; y < oldHeight; y++)
            {
                for (int x = 0; x < oldWidth; x++)
                {
                    if (ColorIdx(oldColors[y * oldWidth + x]) > ColorIdx(newColors[((int)(scaleY * y)) * newWidth + (int)(scaleX * x)]))
                    {
                        //Debug.Log("key color at (" + x.ToString() + ',' + y.ToString() + ')');

                        newColors[((int)(scaleY * y)) * newWidth + (int)(scaleX * x)] = oldColors[y * oldWidth + x];
                    }
                }
            }
            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {

                    if (newColors[y * newWidth + x].a == 0)
                    {
                        newColors[y * newWidth + x] = oldColors[((int)(y / scaleY)) * oldWidth + (int)(x / scaleX)];
                    }
                    //Debug.Log("color at (" + x.ToString() + ',' + y.ToString() + "):" + newColors[y * newWidth + x].ToString());
                }
            }
            return newColors;
        }

        static int ColorIdx(Color32 color)
        {
            for(int i = 0; i < keyColors.Length; i++)
            {
                if (keyColors[i].Equals(color))
                {
                    return i;
                }
            }
            return -1;
        }
        
    }
}