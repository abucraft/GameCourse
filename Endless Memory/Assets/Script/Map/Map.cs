using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TinyJSON;
namespace MemoryTrap
{
    public class RectI
    {
        public int height;
        public int width;
        public int left;
        public int top;
        public int right
        {
            get
            {
                return left + width - 1;
            }
        }
        public int bottom
        {
            get
            {
                return top + height - 1;
            }
        }
        public RectI(int x,int y,int w,int h)
        {
            left = x;
            top = y;
            width = w;
            height = h;
        }
    }

    public class Vector2I
    {
        int _x;
        int _y;
        public int x
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }
        public int y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }
        public Vector2I(int x,int y)
        {
            _x = x;
            _y = y;
        }
    }

    public class Map : MonoBehaviour
    {
        protected Vector2 _location = new Vector2(0,0);
        public MapManager manager;
        public MapBlock[,] map;
        public int level = 0;
        public string style = "normal";
        public List<RectI> roomList = new List<RectI>();
        RectI curArea;
        public void Start()
        {
            
        }

        public void OnDrawGizmos()
        {
            //DrawGizmosMap(0, 100f);
        }
        
        public void DrawGizmosMap(int level,float offset)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    MapBlock bloc = map[i, j];
                    if(bloc == null)
                    {
                        continue;
                    }
                    switch (bloc.type)
                    {
                        case MapBlock.Type.wall:
                            Gizmos.color = Wall.editColor;
                            break;
                        case MapBlock.Type.floor:
                            Gizmos.color = Floor.editColor;
                            break;
                        case MapBlock.Type.wallCorner:
                            Gizmos.color = WallCorner.editColor;
                            break;
                        case MapBlock.Type.door:
                            Gizmos.color = Door.editColor;
                            break;
                    }
                    Gizmos.DrawCube(new Vector3(i + offset, 0, -j), new Vector3(1, 1, 1));
                }
            }
        }
        
        public Vector2 location
        {
            get { return _location; }
            set
            {
                _location = value;
                Vector3 localpos = transform.localPosition;
                localpos.x = _location.x;
                localpos.z = _location.y;
                localpos.y = 0;
                transform.localPosition = localpos;
            }
        }

        public Node Serialize()
        {
            Node cur = Node.NewTable();
            cur["location"] = Node.NewTable();
            cur["location"]["x"] = Node.NewNumber(_location.x);
            cur["location"]["y"] = Node.NewNumber(_location.y);
            cur["level"] = Node.NewInt(level);
            cur["style"] = Node.NewString(style);
            cur["roomList"] = Node.NewArray();
            for(int i = 0; i < roomList.Count; i++)
            {
                Node tmp = Node.NewTable();
                RectI tmpR = roomList[i];
                tmp["left"] = tmpR.left;
                tmp["top"] = tmpR.top;
                tmp["width"] = tmpR.width;
                tmp["height"] = tmpR.height;
                cur["roomList"][i] = tmp;
            }
            cur["map"] = Node.NewArray();
            for(int x = 0; x < map.GetLength(0); x++)
            {
                cur["map"][x] = Node.NewArray();
                for(int y = 0; y < map.GetLength(1); y++)
                {
                    cur["map"][x][y] = map[x, y].Serialize();
                }
            }
            
            return cur;
        }

        public void DeSerialize(Node node)
        {

        }
        
        public void DisableAll()
        {
            for(int x = 0; x < map.GetLength(0); x++)
            {
                for(int y = 0; y < map.GetLength(1); y++)
                {
                    map[x, y].gameObject.SetActive(false);
                }
            }
        }

        public IEnumerator ZoomIn(int frames)
        {
            yield return null;
        }


        public IEnumerator ZoomOut(int frames)
        {
            yield return null;
        }

        public void ShowArea(RectI area)
        {

        }

        public void Update()
        {
            
        }
        
    }

    
}
