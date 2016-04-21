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
        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
