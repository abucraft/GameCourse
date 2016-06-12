using UnityEngine;
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
        public override void CreateObject(Vector2 pos, Transform parent)
        {
            if (gameObject != null)
            {
                return;
            }
            MapManager mpm = MapManager.instance;
            if (mpm != null)
            {
                MapBlockFactory factory = mpm.wallCornerFactory;
                if (factory != null)
                {
                    gameObject = factory.getObject(style);
                    gameObject.transform.parent = parent;
                    gameObject.transform.localPosition = new Vector3(pos.x, 0, pos.y);
                }
            }
        }
    }
}