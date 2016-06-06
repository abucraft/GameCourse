using UnityEngine;
using System.Collections;
namespace MemoryTrap
{
    public class RoomPattern
    {
        public int width;
        public int height;
        public Color32[] color;
        public RoomPattern(Texture2D tex)
        {
            width = tex.width;
            height = tex.height;
            color = tex.GetPixels32();
        }
    }
}