using UnityEngine;
using System.Collections;
namespace MemoryTrap
{
    public class Door : MapBlock
    {
        private bool opened = false;
        public static Color32 editColor = new Color32(0, 0, 255, 255);
        public Door()
        {
            type = Type.door;
        }

        public bool Opened
        {
            get
            {
                return opened;
            }
        }

        public virtual void Open() {
            opened = true;
        }

        
    }
}
