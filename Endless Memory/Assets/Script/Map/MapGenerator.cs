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
    public class MapGenerator : ThreadJob
    {

        public int minRoomHeight;
        public int maxRoomHeight;
        public int minRoomWidth;
        public int maxRoomWidth;
        public int minRoadLength;
        public int maxRoadLength;
        public int maxRoadSize;
        public int maxJudgeTime = 5;
        public int seed;


        public Map map;
        public RoomPattern[] roomPatterns;

        System.Random rand;
        public override void Start()
        {
            Debug.Assert(roomPatterns != null);
            rand = new System.Random(seed);
            base.Start();
        }
        protected override void ThreadFunction()
        {
            GenerateLevelMap();
            Debug.Log("finish map generator");
        }

        protected void GenerateLevelMap()
        {
            
            int initTimes = 10;
            Queue<UsefulArea> areaQue = new Queue<UsefulArea>();
            while (initTimes > 0)
            {
                int height = rand.Next(minRoomHeight, maxRoomHeight+1);
                int width = rand.Next(minRoomWidth, maxRoomWidth+1);
                int initX = rand.Next(0, map.map.GetLength(0));
                int initY = rand.Next(0, map.map.GetLength(1));
                MapBlock.Dir[] dirArray = (MapBlock.Dir[])System.Enum.GetValues(typeof(MapBlock.Dir));
                MapBlock.Dir dir = dirArray[rand.Next(0, dirArray.Length)];
                RectI fArea = CreateArea(width, height, new Vector2I(initX, initY), dir);
                
                if (fArea != null)
                {
                    MakeRoom(fArea);
                    areaQue.Enqueue(new UsefulArea(fArea, maxJudgeTime, "room"));
                    break;
                }
                initTimes--;
            }
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
                        nArea = AddRoad(curArea);
                        if (nArea != null)
                        {
                            UsefulArea nuArea = new UsefulArea(nArea, maxJudgeTime, "road");
                            areaQue.Enqueue(nuArea);
                        }
                        //timeCount--;
                        break;
                    case "room":
                        nArea = AddRoom(curArea);
                        if (nArea != null)
                        {
                            UsefulArea nuArea = new UsefulArea(nArea, maxJudgeTime, "room");
                            areaQue.Enqueue(nuArea);
                        }
                        //timeCount--;
                        break;
                }
                curArea.judgeTimes--;
                if (curArea.judgeTimes >= 0)
                {
                    areaQue.Enqueue(curArea);
                }
                //yield return null;
            }
            for(int x = 0; x < map.map.GetLength(0); x++)
            {
                for(int y = 0; y < map.map.GetLength(1); y++)
                {
                    if(map.map[x,y] == null)
                    {
                        Empty block = new Empty();
                        block.direction = MapBlock.Dir.front;
                        map.map[x, y] = block;
                       
                    }
                    
                }
            }
        }
        

        void FillRoad(RectI area)
        {
            for(int x = area.left; x <= area.right; x++)
            {
                for(int y = area.top; y <= area.bottom; y++)
                {
                    Floor floor = new Floor();
                    floor.direction = MapBlock.Dir.front;
                    map.map[x, y] = floor;
                }
            }
        }

        RectI AddRoad( UsefulArea useArea)
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
                            MapBlock block = map.map[x, y];
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
                    Wall wall = (Wall)map.map[wallLoc.x, wallLoc.y];
                    MapBlock.Dir dir = wall.direction;

                    //move refer to border of area
                    switch (dir)
                    {
                        //这里front的wall是处于area 的bottom处
                        case MapBlock.Dir.front:
                            y = area.bottom;
                            break;
                        case MapBlock.Dir.back:
                            y = area.top;
                            break;
                        
                        case MapBlock.Dir.right:
                            x = area.right;
                            break;
                        case MapBlock.Dir.left:
                            x = area.left;
                            break;
                    }
                    nArea = CreateArea(width, height, new Vector2I(x, y), dir);
                    if (nArea != null)
                    {
                        FillRoad(nArea);
                        map.map[wallLoc.x, wallLoc.y] = null;
                        Door door = new Door();
                        door.direction = dir;
                        map.map[wallLoc.x, wallLoc.y] = door;
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
                    nArea = CreateArea(width, height, new Vector2I(x, y), dir);
                    if(nArea!= null)
                    {
                        FillRoad(nArea);
                    }
                    break;
            }
            return nArea;
        }
        
        RectI AddRoom(UsefulArea useArea)
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

                    nArea = CreateArea(width, height, new Vector2I(x, y), dir);
                    if (nArea != null)
                    {
                        MakeRoom(nArea, new Vector2I(x, y),dir);
                    }
                    break;
            }
            return nArea;
        }

        protected void MakeRoom(RectI area,Vector2I doorPlc = null,MapBlock.Dir roomDir = MapBlock.Dir.front)
        {
            int roomIdx = rand.Next(0, roomPatterns.Length);
            
            Color32[] colorBuffer = RoomPatternScale.ScaleColor(roomPatterns[roomIdx], area.width, area.height);
            
            int width = area.width;
            int height = area.height;
            for(int j = 0; j < height; j++)
            {
                for(int i = 0; i < width; i++)
                {
                    //Debug.Log("place:(" + i.ToString() + ',' + j.ToString() + ')');
                    Color32 tmpColor = colorBuffer[j * width + i];
                    //这里map的z方向和roomPattern的相反
                    //放置墙
                    if(tmpColor.Equals(Wall.editColor))
                    {
                        //判断地图块的方向
                        MapBlock.Dir dir = blockDirection(j, i, width, height, colorBuffer);
                        //Debug.Log("createWall");
                        Wall wall = new Wall();
                        wall.direction = dir;
                        map.map[area.left + i, area.bottom - j] = wall;
                    }
                    //放置墙角
                    if (tmpColor.Equals(WallCorner.editColor))
                    {
                        
                        MapBlock.Dir dir = blockDirection(j, i, width, height, colorBuffer);
                        WallCorner wc = new WallCorner();
                        wc.direction = dir;
                        map.map[area.left + i, area.bottom - j] = wc;
                        //Debug.Log("place wall corner:" +new Vector2I(area.left+i,area.top+j).ToString()+" dir:" + dir.ToString());
                    }
                    //放置地板
                    if (tmpColor.Equals(Floor.editColor))
                    {
                        Floor floor = new Floor();
                        map.map[area.left + i, area.bottom - j] = floor;
                    }
                    //放置地板
                    if (tmpColor.Equals(Empty.editColor))
                    {
                        Empty empty = new Empty();
                        map.map[area.left + i, area.bottom - j] = empty;
                    }
                    //Debug.Log("place:(" + i.ToString() + ',' + j.ToString() + ')');
                    //Debug.Log(maps[level, area.left + i, area.top + j]);
                }
                
            }
            //放置门
            if (doorPlc != null)
            {
                putDoor(area, doorPlc, roomDir);
            }
            map.roomList.Add(area);
        }


        //判断这个块能否放置门
        bool putDoorBlk(Vector2I doorPlc,int dltX,int dltY)
        {
            if (map.map[ doorPlc.x - dltX, doorPlc.y - dltY]!=null 
                && (map.map[doorPlc.x - dltX, doorPlc.y - dltY].type == MapBlock.Type.wall )
                && map.map[doorPlc.x + dltX, doorPlc.y + dltY] != null
                 && (map.map[doorPlc.x + dltX, doorPlc.y + dltY].type == MapBlock.Type.wall ))
            {
                MapBlock oldBlock = map.map[doorPlc.x, doorPlc.y];
                MapBlock.Dir dir = oldBlock.direction;
                Door door = new Door();
                door.direction = dir;
                map.map[doorPlc.x, doorPlc.y] = door;
                return true;
            }
            else
            {
                return false;
            }
        }

        bool putDoor(RectI area, Vector2I doorPlc,MapBlock.Dir roomDir)
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
                            if (putDoorBlk( new Vector2I(x, y), 1, 0))
                                return true;
                        }
                    }
                    for(int x = doorPlc.x; x < area.right; x++)
                    {
                        for (int y = area.bottom; y > area.bottom - area.height / 2; y--)
                        {
                            if (putDoorBlk( new Vector2I(x, y), 1, 0))
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
                            if (putDoorBlk( new Vector2I(x, y), 1, 0))
                                return true;
                        }
                    }
                    for (int x = doorPlc.x; x < area.right; x++)
                    {
                        for (int y = area.top; y < area.bottom - area.height / 2; y++)
                        {
                            if (putDoorBlk( new Vector2I(x, y), 1, 0))
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
                            if (putDoorBlk( new Vector2I(x, y), 0, 1))
                                return true;
                        }
                    }
                    for (int y = doorPlc.y; y > area.top; y--)
                    {
                        for (int x = area.right; x > area.right - area.width / 2; x--)
                        {
                            if (putDoorBlk( new Vector2I(x, y), 0, 1))
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
                            if (putDoorBlk(new Vector2I(x, y), 0, 1))
                                return true;
                        }
                    }
                    for (int y = doorPlc.y; y > area.top; y--)
                    {
                        for (int x = area.left; x < area.right - area.width / 2; x++)
                        {
                            if (putDoorBlk(new Vector2I(x, y), 0, 1))
                                return true;
                        }
                    }
                    break;
            }

            
            return false;
        }

        bool CheckArea(RectI area)
        {
            if (area.top < 0 || area.bottom >= map.map.GetLength(1) || area.left < 0 || area.right >= map.map.GetLength(0))
            {
                return false;
            }
            for (int x = area.left; x <= area.right; x++)
            {
                for (int y = area.top; y <= area.bottom; y++)
                {
                    if (map.map[ x, y] != null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        RectI CreateArea( int width, int height, Vector2I refer, MapBlock.Dir dir)
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
                if (CheckArea(area))
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
                if (CheckArea( area))
                {
                    return area;
                }
                x += dltX;
                y += dltY;
            }
            return null;
        }

        bool Filled(int row,int col,int width,int height,Color32[] colorBuffer)
        {
            if(row<0|| row >= height || col < 0 || col >= width)
            {
                return false;
            }
            else
            {
                return !colorBuffer[row * width + col].Equals(Empty.editColor);
            }
        }

        MapBlock.Dir blockDirection(int row, int col, int width, int height, Color32[] colorBuffer)
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
            if (Filled((row - 1) ,col, width ,height,colorBuffer))
            {
                up = true;
            }
            if (Filled((row + 1), col, width, height, colorBuffer))
            {
                down = true;
            }
            if (Filled(row, col - 1, width, height, colorBuffer))
            {
                left = true;
            }
            if (Filled(row, col + 1, width, height, colorBuffer))
            {
                right = true;
            }
            /*
             * 0    1   0
             * 0    1   1
             * 0    1   0
             */
            if (up && down && right && !left)
            {
                return MapBlock.Dir.left;
            }
            /*
             * 0    1   0
             * 1    1   0
             * 0    1   0
             */
            if (up && down && left && !right)
            {
                return MapBlock.Dir.right;
            }
            /*
             * 0    1   0
             * 1    1   1
             * 0    0   0
             */
            if (left && right && up && !down)
            {
                return MapBlock.Dir.back;
            }
            /*
             * 0    0   0
             * 1    1   1
             * 0    1   0
             */
            if (left && right && down && !up)
            {
                return MapBlock.Dir.front;
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

    }
}
