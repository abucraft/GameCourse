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
            set
            {
                if (opened != value)
                {
                    opened = value;
                    if (gameObject != null)
                    {
                        Animator anime = gameObject.GetComponent<Animator>();
                        anime.SetBool("opened",opened);
                    }
                }
            }
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
                MapBlockFactory factory = mpm.doorFactory;
                if (factory != null)
                {
                    gameObject = factory.getObject(style);
                    gameObject.transform.parent = parent;
                    gameObject.transform.localPosition = new Vector3(pos.x, 0, pos.y);
                    gameObject.GetComponent<Animator>().SetBool("opened", opened);
                }
            }
        }
    }
}
