using UnityEngine;
using System.Collections;
using TinyJSON;

namespace MemoryTrap
{
    public class UpStair : MapBlock
    {
        public Vector2I upPos;
        public UpStair()
        {
            type = Type.upStair;
        }
        public override Node Serialize()
        {
            Node n= base.Serialize();
            if (upPos != null)
            {
                n["upPos"] = Node.NewTable();
                n["upPos"]["x"] = Node.NewInt(upPos.x);
                n["upPos"]["y"] = Node.NewInt(upPos.y);
            }
            return n;
        }

        public override void CreateObject(Vector2 pos, Transform parent)
        {
            MapManager mpm = MapManager.instance;
            if (mpm != null)
            {
                MapBlockFactory factory = mpm.upStairFactory;
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
