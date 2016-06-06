using UnityEngine;
using System.Collections;
namespace MemoryTrap
{
    public class Floor : MapBlock
    {
        public static Color32 editColor = new Color32(189, 135, 5, 255);
        public Floor()
        {
            type = Type.floor;
        }
    }
}