using UnityEngine;
using System.Collections;
namespace MemoryTrap
{
    public class ItemHolder : MonoBehaviour
    {
        public Item item;
        protected Vector2I mapPos;
        protected int level;
        public static int freshCount = 5;
        protected int curFrame = 0;
        public void SetPos(Vector2I position,int lv)
        {
            Vector2 mapLoc = MapManager.instance.maps[lv].location;
            mapPos = position;
            level = lv;
            Vector2 actualPos = (Vector2)mapPos + mapLoc;
            transform.position = new Vector3(actualPos.x, level, actualPos.y);
        }

        void Update()
        {
            curFrame++;
            if (curFrame > freshCount)
            {
                curFrame = 0;
                Map map = MapManager.instance.maps[level];
                if (!map.map[mapPos.x, mapPos.y].inSight)
                {
                    //notice item to destroy object
                    item.inSight = false;
                }
            }
        }
    }
}