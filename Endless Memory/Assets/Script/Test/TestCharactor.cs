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
                Map map = MapManager.instance.maps[curLevel];
                Vector2 pos = map.location;
                pos += new Vector2(mapPos.x, mapPos.y);
                transform.position = new Vector3(pos.x, curLevel, pos.y);
            }

            public void Move(Vector2I target)
            {

            }

            //获取到视野内的方块区域
            public Rect GetCameraRect()
            {
                Ray leftTopR = Camera.main.ScreenPointToRay(new Vector2(0, 0));
                Ray rightTopR = Camera.main.ScreenPointToRay(new Vector2(Screen.width, 0));
                Ray leftBtmR = Camera.main.ScreenPointToRay(new Vector2(0, Screen.height));
                Ray rightBtmR = Camera.main.ScreenPointToRay(new Vector2(Screen.width, Screen.height));
                float fieldOfView = Camera.main.fieldOfView;
                float height = Camera.main.transform.position.y - curLevel;
                float rotateX = Camera.main.transform.rotation.x;
                float angle = rotateX - fieldOfView / 2;
                float length = height / Mathf.Sin(angle);
                Vector3 leftTop = leftTopR.GetPoint(length);
                Vector3 rightTop = rightTopR.GetPoint(length);
                Vector3 leftBtm = leftBtmR.GetPoint(length);
                Vector3 rightBtm = rightBtmR.GetPoint(length);
                float tmpWidth = rightTop.x - leftTop.x;
                float tmpHeight = leftTop.z - leftBtm.z;
                return new Rect(leftBtm.x, leftBtm.z, tmpWidth, tmpHeight);
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