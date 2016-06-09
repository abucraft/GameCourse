using UnityEngine;
using System.Collections;
namespace MemoryTrap
{
    namespace Test
    {
        public class TestCharactor : MonoBehaviour
        {

            public Vector2I mapPos;
            public int curLevel;
            // Use this for initialization
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mousePos = Input.mousePosition;
                    Ray ray = Camera.main.ScreenPointToRay(mousePos);
                    RaycastHit[] hits = Physics.RaycastAll(ray, 1000);
                    if (hits.Length > 0)
                    {
                        MapManager.LogMapBlock(hits[0].transform.position);
                    }
                }
            }

            public void SetPosition()
            {

            }

            public void Move(Vector2I target)
            {

            }

            public void ResetPos()
            {
                Map map = MapManager.instance.maps[0];
                RectI room = map.roomList[0];
                curLevel = map.level;
                Vector2 pos = map.location;
                Vector2 roomPos = new Vector2((room.left + room.right) / 2, (room.top + room.bottom) / 2);
                pos += roomPos;
                mapPos = new Vector2I((room.left + room.right) / 2, (room.top + room.bottom) / 2);
                transform.position = new Vector3(pos.x, curLevel, pos.y);
            }
        }
    }
}