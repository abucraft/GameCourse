using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace MemoryTrap
{
    public class UsefulArea
    {
        public RectI area;
        public int judgeTimes;
        public string type;
        public UsefulArea(RectI rect,int initial,string t)
        {
            type = t;
            area = rect;
            judgeTimes = initial;
        }
    }
    public class MapGenerator : AsyncJob
    {

        public MapBlockFactory wallFactory;
        public MapBlockFactory wallCornerFactory;
        public MapBlockFactory stepFactory;
        public MapBlockFactory doorFactory;
        public MapBlockFactory floorFactory;


        public int minRoomHeight = 3;
        public int maxRoomHeight = 10;
        public int minRoomWidth = 3;
        public int maxRoomWidth = 10;
        public int minRoadLength = 2;
        public int maxRoadLength = 10;
        public int maxRoadSize = 4;
        public int seed = 0;


        public MapBlock[,,] maps;
        public Texture2D[] roomPatterns;
        public List<RectI> roomList = new List<RectI>();

        System.Random rand;
        public override IEnumerator Start()
        {
            Debug.Assert(wallFactory != null);
            Debug.Assert(wallCornerFactory != null);
            Debug.Assert(stepFactory != null);
            Debug.Assert(doorFactory != null);
            Debug.Assert(floorFactory != null);
            Debug.Assert(roomPatterns != null);
            Debug.Assert(wallFactory.type == MapBlock.Type.wall);
            Debug.Assert(wallCornerFactory.type == MapBlock.Type.wallCorner);
            Debug.Assert(stepFactory.type == MapBlock.Type.step);
            Debug.Assert(doorFactory.type == MapBlock.Type.door);
            Debug.Assert(floorFactory.type == MapBlock.Type.floor);
            rand = new System.Random(seed);
            return base.Start();
        }
        protected override IEnumerator AsyncFunction()
        {
            for(int i = 0; i < maps.GetLength(0); i++)
            {
                Debug.Log("generating " + i.ToString());
                yield return GenerateLevelMap(i,"normal");
            }
            Debug.Log("finish map generator");
        }

        protected IEnumerator GenerateLevelMap(int level,string style)
        {
            
            int initTimes = 10;
            Queue<UsefulArea> areaQue = new Queue<UsefulArea>();
            while (initTimes > 0)
            {
                int height = rand.Next(minRoomHeight, maxRoomHeight+1);
                int width = rand.Next(minRoomWidth, maxRoomWidth+1);
                int initX = rand.Next(0, maps.GetLength(1));
                int initY = rand.Next(0, maps.GetLength(2));
                MapBlock.Dir[] dirArray = (MapBlock.Dir[])System.Enum.GetValues(typeof(MapBlock.Dir));
                MapBlock.Dir dir = dirArray[rand.Next(0, dirArray.Length)];
                RectI fArea = CreateArea(level,width, height, new Vector2I(initX, initY), dir);
                
                if (fArea != null)
                {
                    MakeRoom(level, fArea,style);
                    areaQue.Enqueue(new UsefulArea(fArea, 5, "room"));
                    break;
                }
                initTimes--;
                yield return null;
            }
            yield return null;
            //int timeCount = 100;
            while(areaQue.Count!= 0)
            {
                /*if(timeCount == 0)
                {
                    break;
                }*/
                UsefulArea curArea = areaQue.Dequeue();
                //随机选择创建道路或是房间
                string[] seletion = { "road", "room" };
                RectI nArea = null;
                switch (seletion[rand.Next(0, seletion.Length)])
                {
                    case "road":
                        nArea = AddRoad(level, curArea, style);
                        if (nArea != null)
                        {
                            UsefulArea nuArea = new UsefulArea(nArea, 5, "road");
                            areaQue.Enqueue(nuArea);
                        }
                        //timeCount--;
                        break;
                    case "room":
                        nArea = AddRoom(level, curArea, style);
                        if (nArea != null)
                        {
                            UsefulArea nuArea = new UsefulArea(nArea, 5, "room");
                            areaQue.Enqueue(nuArea);
                        }
                        //timeCount--;
                        break;
                }
                curArea.judgeTimes--;
                if (curArea.judgeTimes != 0)
                {
                    areaQue.Enqueue(curArea);
                }
                yield return null;
            }
        }

        void FillRoad(int level, RectI area, string style)
        {
            for(int x = area.left; x <= area.right; x++)
            {
                for(int y = area.top; y <= area.bottom; y++)
                {
                    GameObject floor = floorFactory.getObject(new Vector2((float)x, (float)y), MapBlock.Dir.front, style);
                    maps[level, x, y] = floor.GetComponent<Floor>();
                }
            }
        }

        RectI AddRoad(int level, UsefulArea useArea, string style)
        {
            List<Vector2I> wallList = new List<Vector2I>();
            
            //决定路的长宽
            int width = rand.Next(1, maxRoadSize + 1);
            int height = rand.Next(minRoadLength, maxRoadLength + 1);
            //决定路的方向是竖直还是水平
            switch (rand.Next(0, 2))
            {
                case 0:
                    int tmp = width;
                    width = height;
                    height = tmp;
                    break;
            }
            int x, y;
            //new area
            RectI nArea = null;
            switch (useArea.type)
            {
                case "room":
                    RectI area = useArea.area;
                    for (y = area.top; y <= area.bottom; y++)
                    {
                        for (x = area.left; x <= area.right; x++)
                        {
                            MapBlock block = maps[level, x, y];
                            if (block.type == MapBlock.Type.wall)
                            {
                                wallList.Add(new Vector2I(x, y));
                            }
                        }
                    }
                    int idx = rand.Next(0, wallList.Count);
                    Vector2I wallLoc = wallList[idx];
                    x = wallLoc.x;
                    y = wallLoc.y;
                    Wall wall = (Wall)maps[level, wallLoc.x, wallLoc.y];
                    MapBlock.Dir dir = wall.direction;

                    //move refer to border of area
                    switch (dir)
                    {
                        case MapBlock.Dir.front:
                            y = area.top;
                            break;
                        case MapBlock.Dir.back:
                            y = area.bottom;
                            break;
                        case MapBlock.Dir.right:
                            x = area.right;
                            break;
                        case MapBlock.Dir.left:
                            x = area.left;
                            break;
                    }
                    nArea = CreateArea(level, width, height, new Vector2I(x, y), dir);
                    if (nArea != null)
                    {
                        FillRoad(level, nArea, style);
                        maps[level, wallLoc.x, wallLoc.y] = null;
                        MonoBehaviour.Destroy(wall.gameObject);
                        GameObject door = doorFactory.getObject(new Vector2((float)wallLoc.x, (float)wallLoc.y), dir, style);
                        maps[level, wallLoc.x, wallLoc.y] = door.GetComponent<Door>();
                    }
                    break;
                case "road":
                    area = useArea.area;
                    y = rand.Next(area.top, area.bottom + 1);
                    x = 0;
                    dir = MapBlock.Dir.front;
                    if (y == area.top|| y == area.bottom)
                    {
                        x = rand.Next(area.left, area.right + 1);
                        if (area.height > 1)
                        {
                            if (y == area.top)
                            {
                                dir = MapBlock.Dir.front;
                            }
                            else
                            {
                                dir = MapBlock.Dir.back;
                            }
                        }else
                        {
                            switch (rand.Next(0, 2))
                            {
                                case 0:
                                    dir = MapBlock.Dir.front;
                                    break;
                                case 1:
                                    dir = MapBlock.Dir.back;
                                    break;
                            }
                        }
                    }else
                    {
                        switch (rand.Next(0, 2))
                        {
                            case 0:
                                x = area.left;
                                dir = MapBlock.Dir.left;
                                break;
                            case 1:
                                x = area.right;
                                dir = MapBlock.Dir.right;
                                break;
                        }
                    }
                    nArea = CreateArea(level, width, height, new Vector2I(x, y), dir);
                    if(nArea!= null)
                    {
                        FillRoad(level, nArea, style);
                    }
                    break;
            }
            return nArea;
        }

        
        
        RectI AddRoom(int level,UsefulArea useArea,string style)
        {
            RectI nArea = null;
            int width = rand.Next(minRoomWidth, maxRoomWidth + 1);
            int height = rand.Next(minRoomHeight, maxRoomHeight + 1);

            int x, y;
            RectI area;
            MapBlock.Dir dir= MapBlock.Dir.front;
            switch (useArea.type)
            {
                case "road":
                    area = useArea.area;
                    y = rand.Next(area.top, area.bottom + 1);
                    x = 0;
                    dir = MapBlock.Dir.front;
                    if (y == area.top || y == area.bottom)
                    {
                        x = rand.Next(area.left, area.right + 1);
                        if (area.width != 0)
                        {
                            if (y == area.top)
                            {
                                dir = MapBlock.Dir.front;
                            }
                            else
                            {
                                dir = MapBlock.Dir.back;
                            }
                        }
                        else
                        {
                            switch (rand.Next(0, 2))
                            {
                                case 0:
                                    dir = MapBlock.Dir.front;
                                    break;
                                case 1:
                                    dir = MapBlock.Dir.back;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        switch (rand.Next(0, 2))
                        {
                            case 0:
                                x = area.left;
                                dir = MapBlock.Dir.left;
                                break;
                            case 1:
                                x = area.right;
                                dir = MapBlock.Dir.right;
                                break;
                        }
                    }

                    nArea = CreateArea(level, width, height, new Vector2I(x, y), dir);
                    if (nArea != null)
                    {
                        MakeRoom(level, nArea, style, new Vector2I(x, y),dir);
                    }
                    break;
            }
            return nArea;
        }

        protected void MakeRoom(int level,RectI area,string style,Vector2I doorPlc = null,MapBlock.Dir roomDir = MapBlock.Dir.front)
        {
            int roomIdx = rand.Next(0, roomPatterns.Length);
            
            Texture2D roomPt = new Texture2D(roomPatterns[roomIdx].width, roomPatterns[roomIdx].height,TextureFormat.RGBA32, false);
            Color32[] testColor = roomPatterns[roomIdx].GetPixels32();
            //Debug.Log(testColor[0]);
            
            roomPt.SetPixels32(roomPatterns[roomIdx].GetPixels32());
            RoomPatternScale.Scale(roomPt, area.width, area.height);
            
            Color32[] colorBuffer = roomPt.GetPixels32();
            int width = roomPt.width;
            int height = roomPt.height;
            for(int j = 0; j < height; j++)
            {
                for(int i = 0; i < width; i++)
                {
                    //Debug.Log("place:(" + i.ToString() + ',' + j.ToString() + ')');
                    Color32 tmpColor = colorBuffer[j * width + i];
                    //放置墙
                    if(tmpColor.Equals(Wall.editColor))
                    {
                        //判断地图块的方向
                        MapBlock.Dir dir = blockDirection(j, i, width, height, colorBuffer);
                        
                        GameObject newBlock = wallFactory.getObject(new Vector2((float)(area.left + i), (float)(area.top + j)), dir, style);
                        maps[level, area.left + i, area.top + j] = newBlock.GetComponent<Wall>();
                    }
                    //放置墙角
                    if (tmpColor.Equals(WallCorner.editColor))
                    {
                        MapBlock.Dir dir = blockDirection(j, i, width, height, colorBuffer);
                        
                        GameObject newBlock = wallCornerFactory.getObject(new Vector2((float)(area.left + i), (float)(area.top + j)), dir, style);
                        maps[level, area.left + i, area.top + j] = newBlock.GetComponent<WallCorner>();
                    }
                    //放置地板
                    if (tmpColor.Equals(Floor.editColor))
                    {
                        GameObject newBlock = floorFactory.getObject(new Vector2((float)(area.left + i), (float)(area.top + j)), MapBlock.Dir.front, style);
                        maps[level, area.left + i, area.top + j] = newBlock.GetComponent<Floor>();
                    }
                    //放置地板
                    if (tmpColor.Equals(Empty.editColor))
                    {
                        GameObject newBlock = floorFactory.getObject(new Vector2((float)(area.left + i), (float)(area.top + j)), MapBlock.Dir.front, style);
                        maps[level, area.left + i, area.top + j] = newBlock.GetComponent<Floor>();
                    }
                    //Debug.Log("place:(" + i.ToString() + ',' + j.ToString() + ')');
                    //Debug.Log(maps[level, area.left + i, area.top + j]);
                }
                
            }
            //放置门
            if (doorPlc != null)
            {
                putDoor(level, area, doorPlc, roomDir,style);
            }
            roomList.Add(area);
        }


        //判断这个块能否放置门
        bool putDoorBlk(int level, Vector2I doorPlc,int dltX,int dltY,string style)
        {
            if (maps[level, doorPlc.x - dltX, doorPlc.y - dltY]!=null 
                && (maps[level, doorPlc.x - dltX, doorPlc.y - dltY].type == MapBlock.Type.wall || maps[level, doorPlc.x - dltX, doorPlc.y - dltY].type == MapBlock.Type.wallCorner)
                && maps[level, doorPlc.x + dltX, doorPlc.y + dltY] != null
                 && (maps[level, doorPlc.x + dltX, doorPlc.y + dltY].type == MapBlock.Type.wall || maps[level, doorPlc.x + dltX, doorPlc.y + dltY].type == MapBlock.Type.wallCorner))
            {
                MapBlock oldBlock = maps[level, doorPlc.x, doorPlc.y];
                MapBlock.Dir dir = oldBlock.direction;
                MonoBehaviour.Destroy(oldBlock.gameObject);
                GameObject door = doorFactory.getObject(new Vector2((float)doorPlc.x, (float)doorPlc.y), dir, style);
                maps[level, doorPlc.x, doorPlc.y] = door.GetComponent<Door>();
                return true;
            }
            else
            {
                return false;
            }
        }

        bool putDoor(int level, RectI area, Vector2I doorPlc,MapBlock.Dir roomDir,string style)
        {

            //在左边找到左边适合的放置地点
            switch (roomDir) {
                case MapBlock.Dir.front:
                    for(int x = doorPlc.x; x > area.left; x--)
                    {
                        if(x == area.right)
                        {
                            continue;
                        }
                        for(int y = area.bottom; y > area.bottom - area.height / 2; y--)
                        {
                            if (putDoorBlk(level, new Vector2I(x, y), 1, 0, style))
                                return true;
                        }
                    }
                    for(int x = doorPlc.x; x < area.right; x++)
                    {
                        for (int y = area.bottom; y > area.bottom - area.height / 2; y--)
                        {
                            if (putDoorBlk(level, new Vector2I(x, y), 1, 0, style))
                                return true;
                        }
                    }
                    break;
                case MapBlock.Dir.back:
                    for (int x = doorPlc.x; x > area.left; x--)
                    {
                        if (x == area.right)
                        {
                            continue;
                        }
                        for (int y = area.top; y < area.bottom - area.height / 2; y++)
                        {
                            if (putDoorBlk(level, new Vector2I(x, y), 1, 0, style))
                                return true;
                        }
                    }
                    for (int x = doorPlc.x; x < area.right; x++)
                    {
                        for (int y = area.top; y < area.bottom - area.height / 2; y++)
                        {
                            if (putDoorBlk(level, new Vector2I(x, y), 1, 0, style))
                                return true;
                        }
                    }
                    break;
                case MapBlock.Dir.left:
                    for(int y = doorPlc.y; y < area.bottom; y++)
                    {
                        if(y == area.top)
                        {
                            continue;
                        }
                        for(int x = area.right;x>area.right - area.width / 2; x--)
                        {
                            if (putDoorBlk(level, new Vector2I(x, y), 0, 1, style))
                                return true;
                        }
                    }
                    for (int y = doorPlc.y; y > area.top; y--)
                    {
                        for (int x = area.right; x > area.right - area.width / 2; x--)
                        {
                            if (putDoorBlk(level, new Vector2I(x, y), 0, 1, style))
                                return true;
                        }
                    }
                    break;
                case MapBlock.Dir.right:
                    for (int y = doorPlc.y; y < area.bottom; y++)
                    {
                        if (y == area.top)
                        {
                            continue;
                        }
                        for (int x = area.left; x < area.right - area.width / 2; x++)
                        {
                            if (putDoorBlk(level, new Vector2I(x, y), 0, 1, style))
                                return true;
                        }
                    }
                    for (int y = doorPlc.y; y > area.top; y--)
                    {
                        for (int x = area.left; x < area.right - area.width / 2; x++)
                        {
                            if (putDoorBlk(level, new Vector2I(x, y), 0, 1, style))
                                return true;
                        }
                    }
                    break;
            }

            
            return false;
        }

        bool CheckArea(int level, RectI area)
        {
            if (area.top < 0 || area.bottom >= maps.GetLength(2) || area.left < 0 || area.right >= maps.GetLength(1))
            {
                return false;
            }
            for (int x = area.left; x <= area.right; x++)
            {
                for (int y = area.top; y <= area.bottom; y++)
                {
                    if (maps[level, x, y] != null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        RectI CreateArea(int level, int width, int height, Vector2I refer, MapBlock.Dir dir)
        {
            int dltX = 0;
            int dltY = 0;
            int startX = 0;
            int startY = 0;
            //前提：参考点已被占据
            switch (dir)
            {
                case MapBlock.Dir.front:
                    refer.y--;
                    startX = refer.x - width / 2;
                    startY = refer.y - height +1;
                    dltX = 1;
                    break;
                case MapBlock.Dir.back:
                    refer.y++;
                    startX = refer.x - width / 2;
                    startY = refer.y;
                    dltX = 1;
                    break;
                case MapBlock.Dir.right:
                    refer.x++;
                    startX = refer.x;
                    startY = refer.y - height / 2;
                    dltY = 1;
                    break;
                case MapBlock.Dir.left:
                    refer.x--;
                    startX = refer.x - width +1;
                    startY = refer.y - height / 2;
                    dltY = 1;
                    break;
            }
            //竖直和水平方向滑动area窗口
            int x = startX;
            int y = startY;
            //Debug.Log("refer:(" + refer.x.ToString() + ',' + refer.y.ToString() + ')');
            //Debug.Log(startX.ToString() + ',' + startY.ToString());
            while (x > refer.x - width && y > refer.y - height)
            {
                RectI area = new RectI(x, y, width, height);
                if (CheckArea(level, area))
                {
                    return area;
                }
                x -= dltX;
                y -= dltY;
            }
            x = startX;
            y = startY;
            while (x <= refer.x && y <= refer.y)
            {
                RectI area = new RectI(x, y, width, height);
                if (CheckArea(level, area))
                {
                    return area;
                }
                x += dltX;
                y += dltY;
            }
            return null;
        }

        MapBlock.Dir blockDirection(int row,int col,int width,int height,Color32[] colorBuffer)
        {
            if (row == 0)
            {
                if(col == 0)
                {
                    return MapBlock.Dir.right;
                }
                if(col == width - 1)
                {
                    return MapBlock.Dir.back;
                }
                return MapBlock.Dir.front;
            }
            if(row == height - 1)
            {
                if(col == 0)
                {
                    return MapBlock.Dir.front;
                }
                if(col == width-1)
                {
                    return MapBlock.Dir.left;
                }
                return MapBlock.Dir.back;
            }
            if(col == 0)
            {
                return MapBlock.Dir.left;
            }
            if(col == width -1 )
            {
                return MapBlock.Dir.right;
            }
            if(row>0 && row < height&& col > 0 && col < width)
            {
                //识别pattern中block的方向
                /*
                 * 0       up      0
                 * left    1       right
                 * 0       down    0
                 */
                bool up = false;
                bool down = false;
                bool right = false;
                bool left = false;
                if(!colorBuffer[(row - 1) * width + col].Equals(Empty.editColor))
                {
                    up = true;
                }
                if(!colorBuffer[(row + 1) * width + col].Equals(Empty.editColor))
                {
                    down = true;
                }
                if (!colorBuffer[row * width + col - 1].Equals(Empty.editColor))
                {
                    left = true;
                }
                if (!colorBuffer[row * width + col + 1].Equals(Empty.editColor))
                {
                    right = true;
                }
                /*
                 * 0    1   0
                 * 0    1   1
                 * 0    1   0
                 */
                if (up && down && right&&!left)
                {
                    return MapBlock.Dir.right;
                }
                /*
                 * 0    1   0
                 * 1    1   0
                 * 0    1   0
                 */
                if (up &&down && left && !right)
                {
                    return MapBlock.Dir.left;
                }
                /*
                 * 0    1   0
                 * 1    1   1
                 * 0    0   0
                 */
                if (left && right && up && !down)
                {
                    return MapBlock.Dir.front;
                }
                /*
                 * 0    0   0
                 * 1    1   1
                 * 0    1   0
                 */
                if (left && right && down && !up)
                {
                    return MapBlock.Dir.back;
                }
                /*
                 * 0    1   0
                 * 1    1   0
                 * 0    0   0
                 */
                if (left && up && !right && !down)
                {
                    return MapBlock.Dir.left;
                }
                /*
                 * 0    0   0
                 * 1    1   0
                 * 0    1   0
                 */
                if (left && down && !up && !right)
                {
                    return MapBlock.Dir.back;
                }
                /*
                 * 0    0   0
                 * 0    1   1
                 * 0    1   0
                 */
                if (right && down && !up && !left)
                {
                    return MapBlock.Dir.right;
                }
                /*
                 * 0    1   0
                 * 0    1   1
                 * 0    0   0
                 */
                if (right && up && !down && !left)
                {
                    return MapBlock.Dir.front;
                }
                return MapBlock.Dir.front;
            }
            return MapBlock.Dir.front;
        }


    }
}
