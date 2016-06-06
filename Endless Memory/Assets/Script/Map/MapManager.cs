﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TinyJSON;
namespace MemoryTrap
{
    public class MapManager : MonoBehaviour
    {
        //public Node mapNodes;
        [HideInInspector]
        public Map[] maps;
        [HideInInspector]
        public int curLevel = 0;

        [Range(1, 100000)]
        public int minWidth = 20;
        [Range(1, 100000)]
        public int minLength = 20;
        [Range(1, 100000)]
        public int maxWidth = 100;
        [Range(1, 100000)]
        public int maxLength = 100;
        [Range(1, 10)]
        public int totalLevel = 1;
        [Range(3, 200)]
        public int minRoomHeight = 3;
        [Range(3, 200)]
        public int maxRoomHeight = 20;
        [Range(3, 200)]
        public int minRoomWidth = 3;
        [Range(3, 200)]
        public int maxRoomWidth = 20;
        [Range(2, 100)]
        public int maxRoadSize = 4;
        [Range(2, 100)]
        public int minRoadLength = 4;
        [Range(2, 100)]
        public int maxRoadLength = 10;
        public string[] styles = { "normal" };
        public MapBlockFactory wallFactory;
        public MapBlockFactory emptyFactory;
        public MapBlockFactory upStairFactory;
        public MapBlockFactory downStairFactory;
        public MapBlockFactory doorFactory;
        public MapBlockFactory floorFactory;
        public MapBlockFactory wallCornerFactory;
        public int maxTryTime = 10;
        System.Random rand;
        //自定义pattern显示
        [HideInInspector]
        public Texture2D[] roomPatterns;

        protected RoomPattern[] _roomPatterns;
        
        // Use this for initialization
        void Start()
        {
            Debug.Assert(wallFactory != null);
            Debug.Assert(emptyFactory != null);
            Debug.Assert(upStairFactory != null);
            Debug.Assert(downStairFactory != null);
            Debug.Assert(doorFactory != null);
            Debug.Assert(floorFactory != null);
            Debug.Assert(wallCornerFactory != null);
            _roomPatterns = new RoomPattern[roomPatterns.Length];
            for(int i = 0; i < _roomPatterns.Length; i++)
            {
                _roomPatterns[i] = new RoomPattern(roomPatterns[i]);
            }
            StartCoroutine(GenerateRandomMaps());
        }

        public IEnumerator GenerateRandomMaps()
        {
            if (maps != null && maps.Length != 0)
            {
                Debug.Log("MapManger.GenerateRandomMaps : already have maps");
            }
            else
            {
                maps = new Map[totalLevel];
                MapGenerator[] gens = new MapGenerator[totalLevel];
                if (rand == null)
                    rand = new System.Random(System.DateTime.Now.Millisecond);
                for (int i = 0; i < totalLevel; i++)
                {
                    GameObject gm = new GameObject("map" + i.ToString(), typeof(Map));
                    gm.transform.parent = transform;
                    gm.transform.localPosition = new Vector3(0, i, 0);
                    maps[i] = gm.GetComponent<Map>();
                    int width = rand.Next(minWidth, maxWidth);
                    int length = rand.Next(minLength, maxLength);
                    string style = styles[rand.Next(0, styles.Length)];
                    gens[i] = GenerateMap(i, width, length, style);
                }
                while (true)
                {
                    bool finished = true;
                    for(int i = 0; i < totalLevel; i++)
                    {
                        if(gens[i].IsDone == false)
                        {
                            finished = false;
                        }
                    }
                    yield return null;
                    if (finished)
                    {
                        break;
                    }
                }
                for(int i = 0; i < totalLevel - 1; i++)
                {
                    ConnectMap(maps[i], maps[i + 1]);
                }
            }
        }

        public MapGenerator GenerateMap(int level,int width,int length,string style)
        {
            MapGenerator generator = new MapGenerator();
            maps[level].map = new MapBlock[width, length];
            //初始化生成器
            generator.maxRoadLength = maxRoadLength;
            generator.maxRoadSize = maxRoadSize;
            generator.minRoadLength = minRoadLength;
            generator.minRoomHeight = minRoomHeight;
            generator.maxRoomHeight = maxRoomHeight;
            generator.minRoomWidth = minRoomWidth;
            generator.maxRoomWidth = maxRoomWidth;
       
            generator.seed= System.DateTime.Now.Millisecond;
            generator.roomPatterns = _roomPatterns;
            generator.map = maps[level];
            generator.Start();
            return generator;
        }

        public void ConnectMap(Map lower,Map uper)
        {
            if (rand == null)
                rand = new System.Random(System.DateTime.Now.Millisecond);
            RectI lowRoom = null;
            RectI upRoom = null;
            for (int i = 0; i < maxTryTime; i++)
            {
                RectI _lowRoom = lower.roomList[rand.Next(0, lower.roomList.Count)];
                RectI _upRoom = uper.roomList[rand.Next(0, uper.roomList.Count)];
                if(_lowRoom.width>=5 && _lowRoom.height >=5 && _upRoom.width >= 5 && _upRoom.height >= 5)
                {
                    lowRoom = _lowRoom;
                    upRoom = _upRoom;
                    break;
                }
            }
            if(lowRoom == null|| upRoom == null)
            {
                Debug.Log("MapManager.ConnectMap:can't find approperiate room");
                return;
            }
            List<Vector2I> lowFloorList = new List<Vector2I>();
            for(int x = lowRoom.left+1; x <= lowRoom.right-1; x++)
            {
                for(int y = lowRoom.top+1; y <= lowRoom.bottom-1; y++)
                {
                    if(lower.map[x,y].type == MapBlock.Type.floor && lower.map[x-1,y].type == MapBlock.Type.floor
                        && lower.map[x+1,y].type == MapBlock.Type.floor && lower.map[x,y-1].type == MapBlock.Type.floor
                        && lower.map[x,y+1].type == MapBlock.Type.floor)
                    {
                        lowFloorList.Add(new Vector2I(x, y));
                    }
                }
            }
            Vector2I lowPos = lowFloorList[rand.Next(0, lowFloorList.Count)];
            List<Vector2I> upFloorList = new List<Vector2I>();
            for (int x = upRoom.left + 1; x <= upRoom.right - 1; x++)
            {
                for (int y = upRoom.top + 1; y <= upRoom.bottom - 1; y++)
                {
                    if (uper.map[x, y].type == MapBlock.Type.floor && uper.map[x - 1, y].type == MapBlock.Type.floor
                        && uper.map[x + 1, y].type == MapBlock.Type.floor && uper.map[x, y - 1].type == MapBlock.Type.floor
                        && uper.map[x, y + 1].type == MapBlock.Type.floor)
                    {
                        upFloorList.Add(new Vector2I(x, y));
                    }
                }
            }
            Vector2I upPos = upFloorList[rand.Next(0, upFloorList.Count)];
            //两个阶梯的方向相反
            MapBlock.Dir lowDir = (MapBlock.Dir)rand.Next(0, 4);
            MapBlock.Dir upDir = (MapBlock.Dir)(3 - (int)lowDir);
            UpStair up = new UpStair();
            up.upPos = upPos;
            up.direction = lowDir;
            lower.map[lowPos.x, lowPos.y] = up;
            DownStair down = new DownStair();
            down.lowPos = lowPos;
            down.direction = upDir;
            uper.map[upPos.x, upPos.y] = down;

            //以下一层为准来调整上一层位置
            Vector2 lowLoc = lower.location;
            lowLoc.x += lowPos.x;
            lowLoc.y += lowPos.y;
            Vector2 upLoc = uper.location;
            upLoc.x += upPos.x;
            upLoc.y += upPos.y;
            Vector2 vec = lowLoc - upLoc;
            uper.location = uper.location + vec;
        }

        public Node Serialize()
        {
            Node curNode = Node.NewArray();
            for(int i = 0; i < totalLevel; i++)
            {
                curNode[i] = maps[i].Serialize();
            }
            return curNode;
        }

        public void LoadMap(int level)
        {

        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}