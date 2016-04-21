using UnityEngine;
using System.Collections;
namespace MemoryTrap
{
    public class Wall : MapBlock
    {
        public static Color32 editColor = new Color32(255, 0, 0,255);
        public Wall()
        {
            type = Type.wall;
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
