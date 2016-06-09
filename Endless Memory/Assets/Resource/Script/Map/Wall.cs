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
        public override void CreateObject(Vector2 pos, Transform parent)
        {
            MapManager mpm = MapManager.instance;
            if (mpm != null)
            {
                MapBlockFactory wallFactory = mpm.wallFactory;
                if (wallFactory != null)
                {
                    gameObject = wallFactory.getObject(style);
                    gameObject.transform.parent = parent;
                    gameObject.transform.localPosition = new Vector3(pos.x, 0, pos.y);
                }
            }
        }
    }
}
