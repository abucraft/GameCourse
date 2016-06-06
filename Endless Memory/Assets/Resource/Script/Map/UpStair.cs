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
    }
}
