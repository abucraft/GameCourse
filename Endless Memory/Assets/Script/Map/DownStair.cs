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
    }
}
