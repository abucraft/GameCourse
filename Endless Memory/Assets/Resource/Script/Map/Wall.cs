using UnityEngine;
using System.Collections;
namespace MemoryTrap
{
    public class Wall : MapBlock
    {
        public static Color32 editColor = new Color32(0, 0, 255,255);
        public Wall()
        {
            type = Type.wall;
        }
        
    }
}
