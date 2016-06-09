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

        public bool InSide(int x,int y)
        {
            return x >= left && x <= right && y >= top && y <= bottom;
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
        public bool InArea(RectI rect)
        {
            return _x >= rect.left && _x <= rect.right && _y >= rect.top && _y <= rect.bottom;
        }
        public override string ToString()
        {
            return "(" + _x.ToString() + ',' + _y.ToString() + ')';
        }
    }

    public class Map : MonoBehaviour
    {
        protected Vector2 _location = new Vector2(0,0);
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
        
        public void DrawGizmosMap(float offset)
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
                localpos.y = level;
                transform.localPosition = localpos;
            }
        }

        //将map的style赋值给block
        public virtual void AssignStyle()
        {
            for(int x = 0; x < map.GetLength(0); x++)
            {
                for(int y = 0; y < map.GetLength(1); y++)
                {
                    map[x, y].style = style;
                }
            }
        }

        public Node Serialize()
        {
            Node cur = Node.NewTable();
            cur["location"] = Node.NewTable();
            cur["location"]["x"] = Node.NewNumber(location.x);
            cur["location"]["y"] = Node.NewNumber(location.y);
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

        //从json中恢复
        public void DeSerialize(Node node)
        {
            location = new Vector2((float)(double)(node["location"]["x"]), (float)(double)(node["location"]["y"]));
            level = (int)node["level"];
            style = (string)node["style"];
            List<Node> tmpRoomList = (List<Node>)node["roomList"];
            roomList = new List<RectI>();
            for(int i = 0; i < roomList.Count; i++)
            {
                Node roomNode = tmpRoomList[i];
                roomList.Add(new RectI((int)roomNode["left"], (int)roomNode["top"], (int)roomNode["width"], (int)roomNode["height"]));
            }
            List<Node> colNode = (List<Node>)node["map"];
            map = new MapBlock[colNode.Count, ((List<Node>)colNode[0]).Count];
            for(int x = 0;x< colNode.Count; x++)
            {
                List<Node> rowNode = (List<Node>)colNode[x];
                for(int y = 0; y < rowNode.Count; y++)
                {
                    Node blockNode = rowNode[y];
                    MapBlock.Type type = (MapBlock.Type)(int)blockNode["type"];
                    switch (type)
                    {
                        case MapBlock.Type.door:
                            Door door = new Door();
                            door.DeSerialize(blockNode);
                            map[x, y] = door;
                            break;
                        case MapBlock.Type.downStair:
                            DownStair downStair = new DownStair();
                            downStair.DeSerialize(blockNode);
                            map[x, y] = downStair;
                            break;
                        case MapBlock.Type.upStair:
                            UpStair upStair = new UpStair();
                            upStair.DeSerialize(blockNode);
                            map[x, y] = upStair;
                            break;
                        case MapBlock.Type.empty:
                            Empty empty = new Empty();
                            empty.DeSerialize(blockNode);
                            map[x, y] = empty;
                            break;
                        case MapBlock.Type.floor:
                            Floor floor = new Floor();
                            floor.DeSerialize(blockNode);
                            map[x, y] = floor;
                            break;
                        case MapBlock.Type.wall:
                            Wall wall = new Wall();
                            wall.DeSerialize(blockNode);
                            map[x, y] = wall;
                            break;
                        case MapBlock.Type.wallCorner:
                            WallCorner wallCorner = new WallCorner();
                            wallCorner.DeSerialize(blockNode);
                            map[x, y] = wallCorner;
                            break;
                    }
                }
            }
        }
        
        public void ShowAll()
        {
            for(int x = 0; x < map.GetLength(0); x++)
            {
                for(int y= 0; y < map.GetLength(1); y++)
                {
                    if(map[x,y].gameObject == null)
                    {
                        map[x, y].CreateObject(new Vector2(x, y), transform);
                    }else
                    {
                        map[x, y].gameObject.SetActive(true);
                    }
                }
            }
        }

        public void DisableAll()
        {
            for(int x = 0; x < map.GetLength(0); x++)
            {
                for(int y = 0; y < map.GetLength(1); y++)
                {
                    if(map[x, y].gameObject != null)
                        map[x, y].gameObject.SetActive(false);
                }
            }
        }

        public void DestroyAll()
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    if (map[x, y].gameObject != null)
                        Destroy(map[x, y].gameObject);
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

        public IEnumerator ZoomIn(float time)
        {
            yield return null;
        }


        public IEnumerator ZoomOut(float time)
        {
            yield return null;
        }

        public void ShowArea(RectI area)
        {
            if (area.left < 0)
            {
                area.left = 0;
            }
            if (area.right >= map.GetLength(0))
            {
                area.width = map.GetLength(0) - area.left;
            }
            if (area.top < 0)
            {
                area.top = 0;
            }
            if(area.bottom >= map.GetLength(1))
            {
                area.height = map.GetLength(1) - area.top;
            }
            if(curArea != null)
            {
                for(int x = curArea.left; x <= curArea.right; x++)
                {
                    for(int y = curArea.top; y <= curArea.bottom; y++)
                    {
                        if (!area.InSide(x, y))
                        {
                            if (map[x, y].gameObject != null)
                            {
                                Destroy(map[x, y].gameObject);
                            }
                        }
                    }
                }
            }
            for(int x = area.left; x <= area.right; x++)
            {
                for(int y = area.top; y <= area.bottom; y++)
                {
                    map[x, y].CreateObject(new Vector2(x, y), transform);
                }
            }
        }

        public void Update()
        {
            
        }
        
    }

    
}
