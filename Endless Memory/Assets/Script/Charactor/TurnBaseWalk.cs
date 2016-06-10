using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MemoryTrap
{
    //需要自行获取地图，可以修改AppManager和GameManager
    public abstract class TurnBaseWalk : MonoBehaviour
    {
        public Map getMap()
        {
            GameObject tmp = GameObject.Find("GameObject");
            return tmp.GetComponent<Map>();
        }
        protected Dictionary<Vector2, Vector2> reachableArea;

        //get where the charactor can go
        public abstract Dictionary<Vector2, Vector2> getReachableArea(Vector2 startPos, int step);
        //get the path to a position
        public abstract List<Vector2> getPathTo(Vector2 startPos, Vector2 endPos, int step);
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
    
    public class TurnFlyCharactor : TurnBaseWalk
    {
        override public Dictionary<Vector2, Vector2> getReachableArea(Vector2 startPos, int step)
        {
            return new Dictionary<Vector2, Vector2>();
        }

        override public List<Vector2> getPathTo(Vector2 startPos, Vector2 endPos, int step)
        {
            return new List<Vector2>();
        }
    }

    public class TurnLandCharactor : TurnBaseWalk
    {
        override public Dictionary<Vector2, Vector2> getReachableArea(Vector2 startPos,int step)
        {
            Dictionary<Vector2, Vector2> tmp = new Dictionary<Vector2, Vector2>();
            Queue<KeyValuePair<Vector2, int>> bfs = new Queue<KeyValuePair<Vector2, int>>();
            HashSet<Vector2> reached = new HashSet<Vector2>();

            bfs.Enqueue(new KeyValuePair<Vector2, int>(startPos, 0));
            reached.Add(startPos);
            tmp.Add(startPos, startPos);

            Map map = getMap();

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
            reachableArea = tmp;
            return tmp;
        }

        override public List<Vector2> getPathTo(Vector2 startPos, Vector2 endPos, int step)
        {
            //Dictionary<Vector2, Vector2> reachableArea = getReachableArea(startPos,step);
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
