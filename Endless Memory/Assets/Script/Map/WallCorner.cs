﻿using UnityEngine;
using System.Collections;

namespace MemoryTrap
{
    public class WallCorner : MapBlock
    {
        public static Color32 editColor = new Color32(0,234,255,255);
        public WallCorner()
        {
            type = Type.wallCorner;
        }
        
    }
}