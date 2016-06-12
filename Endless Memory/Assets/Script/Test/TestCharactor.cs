using UnityEngine;
using System.Collections;
namespace MemoryTrap
{
    namespace Test
    {
        public class TestCharactor : MonoBehaviour
        {

            public Vector2I mapPos;
            public int curLevel =0;
            public int sight = 5;
            bool ready = false;
            // Use this for initialization
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {
                //Debug.Log(ready);
                if (ready)
                {
                    UpdatePosition();
                    MapManager.instance.UpdateBlockState(mapPos, sight, curLevel);
                }
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mousePos = Input.mousePosition;
                    Debug.Log(mousePos);
                    Ray ray = Camera.main.ScreenPointToRay(mousePos);
                    RaycastHit[] hits = Physics.RaycastAll(ray, 1000);
                    if (hits.Length > 0)
                    {
                        MapManager.LogMapBlock(hits[0].transform.position);
                    }
                }

            }

            void LateUpdate()
            {
                
                
            }
            public void SetPosition()
            {
                Map map = MapManager.instance.maps[curLevel];
                Vector2 pos = map.location;
                pos += new Vector2(mapPos.x, mapPos.y);
                transform.position = new Vector3(pos.x, curLevel, pos.y);
            }

            public void LogMapPos()
            {
                Debug.Log(mapPos);
            }
            public void UpdatePosition()
            {
                Vector3 pos = transform.position;
                Vector2 tdPos = new Vector2(pos.x, pos.z);
                Vector2 mpPos = tdPos - MapManager.instance.maps[curLevel].location;
                mapPos.x = (int)mpPos.x;
                mapPos.y = (int)mpPos.y;
                pos.y = curLevel;
                transform.position = pos;
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
                ready = true;
            }
        }
    }
}