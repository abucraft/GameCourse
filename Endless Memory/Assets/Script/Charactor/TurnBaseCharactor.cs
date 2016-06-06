using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MemoryTrap
{
    //需要自行获取地图，可以修改AppManager和GameManager
    public abstract class TurnBaseCharactor : MonoBehaviour
    {
        public Vector2 getPos()
        {
            // 假定Pos为（10，10），暂时没有charactor的信息
            return new Vector2(10, 10);
        }
        public int getStep()
        {
            // 假定Step为5，暂时没有charactor的信息
            return 5;
        }

        public Map getMap()
        {
            GameObject tmp = GameObject.Find("GameObject");
            return tmp.GetComponent<Map>();
        }

        //get where the charactor can go
        public abstract Dictionary<Vector2, Vector2> getReachableArea();
        //get the path to a position
        public abstract List<Vector2> getPathTo(Vector2 endPos);
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
    
    public class TurnFlyCharactor : TurnBaseCharactor
    {
        override public Dictionary<Vector2, Vector2> getReachableArea()
        {
            return new Dictionary<Vector2, Vector2>();
        }

        override public List<Vector2> getPathTo(Vector2 endPos)
        {
            return new List<Vector2>();
        }
    }

    public class TurnLandCharactor : TurnBaseCharactor
    {
        override public Dictionary<Vector2, Vector2> getReachableArea()
        {
            Dictionary<Vector2, Vector2> tmp = new Dictionary<Vector2, Vector2>();
            Queue<KeyValuePair<Vector2, int>> bfs = new Queue<KeyValuePair<Vector2, int>>();
            HashSet<Vector2> reached = new HashSet<Vector2>();

            bfs.Enqueue(new KeyValuePair<Vector2, int>(getPos(), 0));
            reached.Add(getPos());
            tmp.Add(getPos(), getPos());

            Map map = getMap();
            int step = getStep();

            while (bfs.Count != 0)
            {
                KeyValuePair<Vector2, int> now = bfs.Dequeue();
                Vector2 posNow = now.Key;
                int stepNow = now.Value;
                int x = (int)posNow.x;
                int y = (int)posNow.y;
                MapBlock mapBlock = map.map[x, y];
                int[] next = { 1, -1 };
                if (stepNow < step)
                {
                    for (int i = 0; i < 2; i++)
                        for (int j = 0; j < 2; j++)
                        {
                            int xNew = x + next[i];
                            int yNew = y + next[j];
                            if (xNew > -1 && xNew< map.map.GetLength(0)
                                && yNew > -1 && yNew <map.map.GetLength(1))
                            {
                                MapBlock mapBlockNew = map.map[xNew, yNew];

                                if (mapBlockNew.type == MapBlock.Type.upStair
                                    || mapBlockNew.type == MapBlock.Type.downStair
                                    || mapBlockNew.type == MapBlock.Type.floor
                                    || ( mapBlockNew.type == MapBlock.Type.door && ((Door)mapBlockNew).Opened))
                                {
                                    Vector2 posNew = new Vector2(xNew, yNew);
                                    if (!reached.Contains(posNew))
                                    {
                                        reached.Add(posNew);
                                        bfs.Enqueue(new KeyValuePair<Vector2, int>(posNew, stepNow + 1));
                                        tmp.Add(posNew, posNow);
                                    }
                                }
                            }
                        }
                }
            }
            return tmp;
        }

        override public List<Vector2> getPathTo(Vector2 endPos)
        {
            Dictionary<Vector2, Vector2> reachableArea = getReachableArea();
            List<Vector2> path = new List<Vector2>();

            Vector2 nowPos = endPos;
            while(reachableArea[nowPos] != nowPos)
            {
                path.Add(nowPos);
                nowPos = reachableArea[nowPos];
            }
            return path;
        }
    }
}
