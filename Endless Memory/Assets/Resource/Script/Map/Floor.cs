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


        public override void CreateObject(Vector2 pos, Transform parent)
        {
            MapManager mpm = MapManager.instance;
            if (mpm != null)
            {
                MapBlockFactory factory = mpm.floorFactory;
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