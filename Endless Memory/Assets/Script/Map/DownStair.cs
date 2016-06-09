using UnityEngine;
using System.Collections;
using TinyJSON;

namespace MemoryTrap
{
    public class DownStair : MapBlock
    {
        public Vector2I lowPos;
        public DownStair()
        {
            type = Type.downStair;
        }
        public override Node Serialize()
        {
            Node n = base.Serialize();
            if (lowPos != null)
            {
                n["lowPos"] = Node.NewTable();
                n["lowPos"]["x"] = Node.NewInt(lowPos.x);
                n["lowPos"]["y"] = Node.NewInt(lowPos.y);
            }
            return n;
        }


        public override void CreateObject(Vector2 pos, Transform parent)
        {
            MapManager mpm = MapManager.instance;
            if (mpm != null)
            {
                MapBlockFactory factory = mpm.downStairFactory;
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
