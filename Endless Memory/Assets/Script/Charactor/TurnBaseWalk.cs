using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        public Dictionary<Vector2,TurnBaseCharactor> getLocationCharactor()
        {
            return GameObject.Find("GameManager").GetComponent<GameManager>().locationCharactors;
        }

        protected Dictionary<Vector2, Vector2> reachableArea;
        public List<Vector2> reachableCharator;

        public bool reachableMapBlock(Vector2 pos, Map map)
        {
            int x = (int)pos.x;
            int y = (int)pos.y;

            if (x < 0 || x >= map.map.GetLength(0)
                || y < 0 || y >= map.map.GetLength(1))
                return false;

            MapBlock mapBlock = map.map[x, y];

            return
                ((mapBlock.type == MapBlock.Type.door && ((Door)mapBlock).Opened) ||
                mapBlock.type == MapBlock.Type.downStair ||
                mapBlock.type == MapBlock.Type.upStair ||
                mapBlock.type == MapBlock.Type.floor);
        }

        //get where the charactor can go
        public abstract Dictionary<Vector2, Vector2> getReachableArea(Vector2 startPos, int step);
        //get the path to a position
        public abstract List<Vector2> getPathTo(Vector2 startPos, Vector2 endPos);
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
    
    public class TurnMainWalk : TurnBaseWalk
    {
        override public Dictionary<Vector2, Vector2> getReachableArea(Vector2 startPos, int step)
        {
            Dictionary<Vector2, Vector2> tmp = new Dictionary<Vector2, Vector2>();
            Queue<KeyValuePair<Vector2, int>> bfs = new Queue<KeyValuePair<Vector2, int>>();
            HashSet<Vector2> reached = new HashSet<Vector2>();
            
            bfs.Enqueue(new KeyValuePair<Vector2, int>(startPos, 0));
            reached.Add(startPos);
            tmp.Add(startPos, startPos);

            Map map = getMap();
            Dictionary<Vector2, TurnBaseCharactor> locationCharactor = getLocationCharactor();

            while (bfs.Count != 0)
            {
                KeyValuePair<Vector2, int> now = bfs.Dequeue();
                Vector2 posNow = now.Key;
                int stepNow = now.Value;
                int x = (int)posNow.x;
                int y = (int)posNow.y;
                MapBlock mapBlock = map.map[x, y];
                int[] next = { 1, -1, 0 };
                if (stepNow < step)
                {
                    for (int i = 0; i < 3; i++)
                        for (int j = 0; j < 3; j++)
                        {
                            if (next[i] != 0 && next[j] != 0) continue;
                            if (next[i] == 0 && next[j] == 0) continue;
                            int xNew = x + next[i];
                            int yNew = y + next[j];
                            if (xNew > -1 && xNew < map.map.GetLength(0)
                                && yNew > -1 && yNew < map.map.GetLength(1))
                            {
                                MapBlock mapBlockNew = map.map[xNew, yNew];
                                Vector2 posNew = new Vector2(xNew, yNew);
                                if (!locationCharactor.ContainsKey(new Vector2(xNew, yNew)))
                                {
                                    if
                                    (mapBlockNew.type == MapBlock.Type.upStair
                                    || mapBlockNew.type == MapBlock.Type.downStair
                                    || mapBlockNew.type == MapBlock.Type.floor
                                    || (mapBlockNew.type == MapBlock.Type.door && ((Door)mapBlockNew).Opened))
                                    {
                                        if (!reached.Contains(posNew))
                                        {
                                            reached.Add(posNew);
                                            bfs.Enqueue(new KeyValuePair<Vector2, int>(posNew, stepNow + 1));
                                            tmp.Add(posNew, posNow);
                                        }
                                    }
                                }
                                else
                                {
                                    reachableCharator.Add(posNew);
                                }
                            }
                        }
                }
            }
            reachableArea = tmp;
            return tmp;
        }

        override public List<Vector2> getPathTo(Vector2 startPos, Vector2 endPos)
        {
            //Dictionary<Vector2, Vector2> reachableArea = getReachableArea(startPos,step);
            List<Vector2> path = new List<Vector2>();
            
            Vector2 nowPos = endPos;
            while (!reachableArea[nowPos].Equals(nowPos))
            {
                path.Add(nowPos);
                nowPos = reachableArea[nowPos];
            }
            path.Add(nowPos);
            path.Reverse();
            return path;
        }
    }

    public class TurnEnemyWalk : TurnBaseWalk
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
            Dictionary<Vector2, TurnBaseCharactor> locationCharactor = getLocationCharactor();

            while (bfs.Count != 0)
            {
                KeyValuePair<Vector2, int> now = bfs.Dequeue();
                Vector2 posNow = now.Key;
                int stepNow = now.Value;
                int x = (int)posNow.x;
                int y = (int)posNow.y;
                MapBlock mapBlock = map.map[x, y];
                int[] next = { 1, -1, 0 };
                if (stepNow < step)
                {
                    for (int i = 0; i < 3; i++)
                        for (int j = 0; j < 3; j++)
                        {
                            if (next[i] != 0 && next[j] != 0) continue;
                            if (next[i] == 0 && next[j] == 0) continue;
                            int xNew = x + next[i];
                            int yNew = y + next[j];
                            if (xNew > -1 && xNew< map.map.GetLength(0)
                                && yNew > -1 && yNew <map.map.GetLength(1))
                            {
                                MapBlock mapBlockNew = map.map[xNew, yNew];
                                Vector2 posNew = new Vector2(xNew, yNew);
                                if (!locationCharactor.ContainsKey(new Vector2(xNew, yNew)))
                                {
                                    if (mapBlockNew.type == MapBlock.Type.upStair
                                    || mapBlockNew.type == MapBlock.Type.downStair
                                    || mapBlockNew.type == MapBlock.Type.floor
                                    || (mapBlockNew.type == MapBlock.Type.door && ((Door)mapBlockNew).Opened))
                                    {

                                        if (!reached.Contains(posNew))
                                        {
                                            reached.Add(posNew);
                                            bfs.Enqueue(new KeyValuePair<Vector2, int>(posNew, stepNow + 1));
                                            tmp.Add(posNew, posNow);
                                        }
                                    }
                                }
                                else reachableCharator.Add(posNew);
                            }
                        }
                }
            }
            reachableArea = tmp;
            return tmp;
        }

        override public List<Vector2> getPathTo(Vector2 startPos, Vector2 endPos)
        {
            // A*算法
            Map map = getMap();
            Dictionary<Vector2, TurnBaseCharactor> locationCharactor = getLocationCharactor();

            HashSet<Vector2> close = new HashSet<Vector2>();
            Dictionary<Vector2, int> open = new Dictionary<Vector2, int>();
            Dictionary<Vector2, Vector2> tmp = new Dictionary<Vector2, Vector2>();
           
            int G = 0;
            int H = (int)(System.Math.Abs(startPos.x-endPos.x) + System.Math.Abs(startPos.y-endPos.y));
            int F = G + H;
            open.Add(startPos,F);
            tmp.Add(startPos, startPos);

            var sortedOpen = from pair in open orderby pair.Value select pair;

            while (open.Count >0 && !sortedOpen.First().Key.Equals(endPos))
            {
                Vector2 nowPos = sortedOpen.First().Key;
                int x = (int)nowPos.x;
                int y = (int)nowPos.y;
                int nowH = (int)(System.Math.Abs(nowPos.x - endPos.x) + System.Math.Abs(nowPos.y - endPos.y));
                int nowF = sortedOpen.First().Value;
                int nowG = nowF - nowH;

                int[] next = { 1, -1, 0 };
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                    {
                        if (next[i] != 0 && next[j] != 0) continue;
                        if (next[i] == 0 && next[j] == 0) continue;
                        int xNew = x + next[i];
                        int yNew = y + next[j];
                        if (xNew > -1 && xNew < map.map.GetLength(0)
                            && yNew > -1 && yNew < map.map.GetLength(1))
                        {
                            MapBlock mapBlockNew = map.map[xNew, yNew];
                            if ((xNew == (int)endPos.x && yNew == (int)endPos.y)
                                || !locationCharactor.ContainsKey(new Vector2(xNew, yNew)) &&
                                    (mapBlockNew.type == MapBlock.Type.upStair
                                    || mapBlockNew.type == MapBlock.Type.downStair
                                    || mapBlockNew.type == MapBlock.Type.floor
                                    || (mapBlockNew.type == MapBlock.Type.door && ((Door)mapBlockNew).Opened)))
                            {
                                Vector2 newPos = new Vector2(xNew, yNew);
                                int newG = nowG + 1;
                                int newH = (int)(System.Math.Abs(newPos.x - endPos.x) + System.Math.Abs(newPos.y - endPos.y));
                                int newF = newG + newH;
                                if (!close.Contains(newPos))
                                {
                                    if (open.ContainsKey(newPos))
                                    {
                                        if (newF < open[newPos])
                                        {
                                            open[newPos] = newF;
                                            tmp[newPos] = nowPos;
                                        }
                                    }
                                    else
                                    {
                                        open.Add(newPos, newF);
                                        tmp.Add(newPos, nowPos);
                                    }
                                }
                            }
                        }
                    }
                open.Remove(nowPos);
                close.Add(nowPos);

                sortedOpen = from pair in open orderby pair.Value select pair;
            }

            List<Vector2> path = new List<Vector2>();

            if (!tmp.ContainsKey(endPos)) return path;

            Vector2 _nowPos = endPos;
            while (!tmp[_nowPos].Equals(_nowPos))
            {
                path.Add(_nowPos);
                _nowPos = tmp[_nowPos];
            }
            path.Add(_nowPos);
            path.Reverse();
            return path;
        }
    }
}
