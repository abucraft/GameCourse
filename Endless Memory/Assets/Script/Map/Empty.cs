using UnityEngine;
using System.Collections;
namespace MemoryTrap
{
    public class Empty : MapBlock
    {
        public static Color32 editColor = new Color32(255,255,255,255);

        public Empty()
        {
            type = Type.empty;
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
                MapBlockFactory factory = mpm.emptyFactory;
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
