using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace MemoryTrap
{
    public class MainCharactor : TurnBaseCharactor
    {
        public enum State
        {
            idle,
            findWay,
            animating
        }
        public int sight = 5;
        private MapBlock _focusBlk;
        public State state;
        private int availableStep;
        private List<Vector2> availableArea = new List<Vector2>();

        public MapBlock focusBlk
        {
            get
            {
                return _focusBlk;
            }
            set
            {
                if (_focusBlk != null)
                {
                    _focusBlk.selected = false;
                }
                _focusBlk = value;
                if (_focusBlk != null)
                {
                    _focusBlk.selected = true;
                }
            }
        }
        public override void BeginTurn()
        {
            Debug.Log("begin turn");
            base.BeginTurn();
            EnterIdle(true);
        }

        public void EnterIdle(bool reset)
        {
            Debug.Log("left steps:" + availableStep);
            state = State.idle;
            //重置移动点数
            if (reset)
            {
                availableStep = step;
            }
            //如果step为0，结束回合
            if(availableStep == 0)
            {
                turnOver = true;
                return;
            }
            ShowAvaliableArea();
        }
        // Update is called once per frame
        void Update()
        {
            if (!turnOver)
            {
                if(state == State.idle)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector3 mousePos = Input.mousePosition;
                        Ray ray = Camera.main.ScreenPointToRay(mousePos);
                        RaycastHit[] hits = Physics.RaycastAll(ray, 100);
                        //根据选中的物体采取行动
                        if (hits.Length > 0)
                        {
                            //如果选中的是地图块
                            Vector2I blkPos = MapManager.instance.getMapBlockPos(hits[0].transform.position, curLevel);
                            if(blkPos != null)
                            {
                                MapBlock blk = MapManager.instance.maps[curLevel].map[blkPos.x, blkPos.y];
                                
                                //如果是可以移动的位置
                                if (blk.available)
                                {
                                    focusBlk = blk;
                                    StartCoroutine(goPath(walk.getPathTo(position, (Vector2)blkPos)));
                                    DisabelAvailableArea();
                                }
                            }
                        }
                    }
                }
                if (false)
                {
                    Vector3 mousePos = Input.mousePosition;
                    Debug.Log(mousePos);
                    Ray ray = Camera.main.ScreenPointToRay(mousePos);
                    RaycastHit[] hits = Physics.RaycastAll(ray, 1000);
                    if (hits.Length > 0)
                    {
                        Vector2I blkPos = MapManager.instance.LogMapBlock(hits[0].transform.position);
                        MapBlock blk = MapManager.instance.maps[curLevel].map[blkPos.x, blkPos.y];
                        if (focusBlk != null)
                        {
                            focusBlk.selected = false;
                            focusBlk.HideInfo();
                        }
                        blk.selected = true;
                        blk.DisplayInfo(blkPos.ToString() + '\n');
                        focusBlk = blk;
                    }

                }
            }

        }

        public void ShowAvaliableArea()
        {
            //Debug.Log("show available area");
            
            walk.curLevel = curLevel;
            Dictionary<Vector2,Vector2> area = walk.getReachableArea(position, availableStep);
            availableArea.Clear();
            foreach(Vector2 pos in area.Keys)
            {
                MapManager.instance.maps[curLevel].map[(int)pos.x, (int)pos.y].available = true;
                availableArea.Add(pos);
            }
            /*Debug.Log(position);
            Debug.Log("available area:"+availableArea.Count);
            foreach(Vector2 pos in availableArea)
            {
                Debug.Log(pos);
            }*/

        }

        public void DisabelAvailableArea()
        {
            foreach(Vector2 pos in availableArea)
            {
                MapManager.instance.maps[curLevel].map[(int)pos.x, (int)pos.y].available = false;
            }
            availableArea.Clear();
        }

        public void SetPosition()
        {
            Map map = MapManager.instance.maps[curLevel];
            Vector2 pos = map.location;
            pos += position;
            transform.position = new Vector3(pos.x, curLevel, pos.y);
        }

        public void LogMapPos()
        {
            Debug.Log(position);
        }

        public void Move(Vector2I target)
        {

        }



        public void ResetPos()
        {
            Map map = MapManager.instance.maps[0];
            RectI room = map.roomList[0];
            curLevel = map.level;
            Vector2 pos = map.location;
            Vector2 roomPos = new Vector2((room.left + room.right) / 2, (room.top + room.bottom) / 2);
            pos += roomPos;
            position = new Vector2((room.left + room.right) / 2, (room.top + room.bottom) / 2);
            transform.position = new Vector3(pos.x, curLevel, pos.y);
            MapManager.instance.UpdateBlockState(position, sight, curLevel);
        }

        public IEnumerator goPath(List<Vector2> path)
        {
            state = State.animating;
            //第一个path是自己所在的block
            for (int i = 1; i < path.Count; i++)
            {
                Vector2 next = path[i];
                for (int j = 0; j < moveFrame; j++)
                {
                    float x = Mathf.Lerp(position.x, next.x, j / (float)moveFrame);
                    float y = Mathf.Lerp(position.y, next.y, j / (float)moveFrame);
                    transform.position = new Vector3(x, curLevel, y);
                    yield return null;
                }
                position = next;
                MapManager.instance.UpdateBlockState(position, sight, curLevel);
                availableStep--;
                yield return null;
            }
            //移动结束，进入idle状态,不重置移动点数
            EnterIdle(false);
        }

    }
}