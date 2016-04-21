using UnityEngine;
using System.Collections;
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
                return left + width;
            }
        }
        public int bottom
        {
            get
            {
                return top + height;
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
        [Range(1, 100000)]
        public int width = 1;
        [Range(1, 100000)]
        public int length = 1;
        [Range(1,10)]
        public int totalLevel = 1;
        MapBlock[,,] maps;
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
        public MapBlockFactory wallFactory;
        public MapBlockFactory stepFactory;
        public MapBlockFactory doorFactory;
        public MapBlockFactory floorFactory;
        public MapBlockFactory wallCornerFactory;
        
        //自定义pattern显示
        [HideInInspector]
        public Texture2D[] roomPatterns;

        MapGenerator generator;
        public void Start()
        {
            generator = new MapGenerator();
            maps = new MapBlock[totalLevel, width, length];
            //初始化生成器
            generator.maps = maps;
            generator.maxRoadSize = maxRoadSize;
            generator.minRoomHeight = minRoomHeight;
            generator.maxRoomHeight = maxRoomHeight;
            generator.minRoomWidth = minRoomWidth;
            generator.maxRoomWidth = maxRoomWidth;
            generator.wallFactory = wallFactory;
            generator.stepFactory = stepFactory;
            generator.doorFactory = doorFactory;
            generator.floorFactory = floorFactory;
            generator.wallCornerFactory = wallCornerFactory;
            generator.roomPatterns = roomPatterns;
            StartCoroutine(generator.Start());
            generator.Start();
            
        }
        
        
        public void Update()
        {
            
        }
        
    }

    
}
