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
            waitUI,
            findWay,
            animating
        }

        public delegate void ActionCallback();

        public int sight = 5;
        private MapBlock _focusBlk;
        public State state;
        private int availableStep;
        private List<Vector2> availableArea = new List<Vector2>();
        public int upDownFrame;
        //目前只有15个格子
        public ItemPack[] items = new ItemPack[15];

        public override int hp
        {
            get
            {
                return _hp;
            }

            set
            {
                _hp = value;
                UIManager.instance.hpBar.totalValue = _hp;
            }
        }

        public override int curHp
        {
            get
            {
                return _curHp;
            }

            set
            {
                _curHp = value;
                UIManager.instance.hpBar.restValue = _curHp;
            }
        }

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
            //Debug.Log("begin turn");
            base.BeginTurn();
            UIManager.instance.loading.SetActive(false);
            EnterIdle(true);
        }

        public void EndTurn()
        {
            turnOver = true;
            DisabelAvailableArea();
            UIManager.instance.loading.SetActive(true);
        }

        public void EnterIdle(bool reset)
        {
            //Debug.Log("left steps:" + availableStep);
            state = State.idle;
            //重置移动点数
            if (reset)
            {
                availableStep = step;
            }
            //如果step为0，结束回合
            if(availableStep == 0)
            {
                EndTurn();
                return;
            }
            ShowAvaliableArea();
        }

        //拾取物品
        public void CollectItem()
        {
            //拾取物品
            Dictionary<Vector2, ItemPack> itemDic = GameManager.instance.levelItemPacks[curLevel];
            if (itemDic.ContainsKey(position))
            {
                ItemPack itmPack = itemDic[position];
                UIManager.instance.CreateCollectionHint((string)itmPack.item.info["name"] + " X " + itmPack.count.ToString());
                //遍历物品栏查看是否已存在这个物品
                bool contain = false;
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] == null)
                    {
                        continue;
                    }
                    if (items[i].item.GetType() == itmPack.item.GetType())
                    {
                        contain = true;
                        items[i].count += itmPack.count;
                    }
                }
                if (!contain)
                {
                    //使用一个新槽位来存放物品
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (items[i] == null)
                        {
                            items[i] = itmPack;
                            itmPack.item.inSight = false;
                            break;
                        }
                    }
                }
                //从dictionary中删除物品
                itemDic.Remove(position);
            }
        }

        void OnEnable()
        {
            curHp = _curHp;
            hp = _hp;
        }

        // Update is called once per frame
        void Update()
        {
            if (!turnOver)
            {
                //拾取物品
                CollectItem();
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
                            //如果选中的是Npc
                            if (hits[0].collider.GetComponent<NpcCharactor>() != null)
                            {
                                state = State.animating;
                                string[] options = new string[2];
                                options[0] = StringResource.talk;
                                options[1] = StringResource.cancel;
                                Vector3 dialogPos = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z + 3);
                                UIManager.instance.CreateSelectDlg(Camera.main.WorldToScreenPoint(dialogPos), "", options, (int idx) =>
                                {
                                    if (idx == 0)
                                    {
                                        StartConversation(hits[0].collider.GetComponent<NpcCharactor>());
                                        return;
                                    }
                                    EnterIdle(false);
                                });
                                return;
                            }
                            //如果选中的是Enemy
                            if (hits[0].collider.GetComponent<EnemyCharactor>() != null)
                            {
                                state = State.animating;
                                EnemyCharactor enemy = hits[0].collider.GetComponent<EnemyCharactor>();
                                if (!IsNearBy(enemy.position))
                                {
                                    string[] options = new string[1];
                                    options[0] = StringResource.cancel;
                                    UIManager.instance.CreateInfomationDlg(enemy, options, (int idx) =>
                                      {
                                          EnterIdle(false);
                                      });
                                }
                                else
                                {
                                    //在附近可以选择开战
                                    string[] options = new string[2];
                                    options[0] = StringResource.fight;
                                    options[1] = StringResource.cancel;
                                    UIManager.instance.CreateInfomationDlg(enemy, options, (int idx) =>
                                      {
                                          EnterIdle(false);
                                      });
                                }
                                return;
                            }
                            //如果选中的是地图块
                            Vector2I blkPos = MapManager.instance.getMapBlockPos(hits[0].transform.position, curLevel);
                            if (blkPos != null)
                            {
                                //如果在自己脚下，什么事也不做
                                if (blkPos.x == (int)position.x && blkPos.y == (int)position.y)
                                {
                                    return;
                                }
                                MapBlock blk = MapManager.instance.maps[curLevel].map[blkPos.x, blkPos.y];
                                //如果是门
                                if (blk.type == MapBlock.Type.door)
                                {
                                    focusBlk = blk;
                                    Door dor = (Door)blk;
                                    //如果门是关闭状态并且在自己附近
                                    if (!dor.Opened && IsNearBy((Vector2)blkPos))
                                    {
                                        string[] options = new string[2];
                                        options[0] = StringResource.openDoor;
                                        options[1] = StringResource.cancel;
                                        state = State.waitUI;
                                        //取消显示可移动的块
                                        DisabelAvailableArea();
                                        Vector3 dialogPos = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z + 3);
                                        UIManager.instance.CreateSelectDlg(Camera.main.WorldToScreenPoint(dialogPos), StringResource.door, options, (int idx) =>
                                            {
                                                if (idx == 0)
                                                {
                                                    dor.Opened = true;
                                                    EndTurn();
                                                    MapManager.instance.UpdateBlockState(position, sight, curLevel);
                                                    return;
                                                }
                                                EnterIdle(false);
                                            });
                                    }
                                    //如果门是开启状态并且可以到达
                                    else if(dor.Opened&&dor.available)
                                    {
                                        string[] options = new string[3];
                                        options[0] = StringResource.closeDoor;
                                        options[1] = StringResource.move;
                                        options[2] = StringResource.cancel;
                                        //取消显示可移动的块
                                        DisabelAvailableArea();
                                        Vector3 dialogPos = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z + 3);
                                        UIManager.instance.CreateSelectDlg(Camera.main.WorldToScreenPoint(dialogPos), StringResource.door, options, (int idx) =>
                                            {
                                                if (idx == 0)
                                                {
                                                    //关门
                                                    if (IsNearBy((Vector2)blkPos))
                                                    {
                                                        dor.Opened = false;
                                                        //开关门消耗所有移动点
                                                        EndTurn();
                                                        MapManager.instance.UpdateBlockState(position, sight, curLevel);
                                                        return;
                                                    }
                                                    //走去关门
                                                    else
                                                    {
                                                        List<Vector2> pathlist = walk.getPathTo(position, (Vector2)blkPos);
                                                        pathlist.RemoveAt(pathlist.Count - 1);
                                                        StartCoroutine(goPathAndAction(pathlist, () =>
                                                        {
                                                            dor.Opened = false;
                                                            EndTurn();
                                                            MapManager.instance.UpdateBlockState(position, sight, curLevel);
                                                            return;
                                                        }));
                                                        return;
                                                    }

                                                }
                                                if (idx == 1)
                                                {
                                                    //移动
                                                    StartCoroutine(goPath(walk.getPathTo(position, (Vector2)blkPos)));
                                                    return;
                                                }
                                                EnterIdle(false);
                                            });
                                    }
                                    return;
                                }
                                else if(blk.type == MapBlock.Type.upStair||blk.type == MapBlock.Type.downStair)
                                {
                                    //检查角色位置是否是楼梯正对位
                                    int dx = 0;
                                    int dy = 0;
                                    
                                    switch (blk.direction)
                                    {
                                        case MapBlock.Dir.front:
                                            /*
                                             * -
                                             * P
                                             */
                                            if((int)position.y != blkPos.y-1||(int)position.x != blkPos.x)
                                            {
                                                return;
                                            }
                                            dy = 1;
                                            break;
                                        case MapBlock.Dir.back:
                                            /*
                                             * P
                                             * -
                                             */
                                            if ((int)position.y != blkPos.y + 1 || (int)position.x != blkPos.x)
                                            {
                                                return;
                                            }
                                            dy = -1;
                                            break;
                                        case MapBlock.Dir.left:
                                            /*
                                             *- P 
                                             */
                                            if ((int)position.x != blkPos.x + 1 || (int)position.y != blkPos.y)
                                            {
                                                return;
                                            }
                                            dx = -1;
                                            break;
                                        case MapBlock.Dir.right:
                                            /*
                                             * P - 
                                             */
                                            if ((int)position.x != blkPos.x - 1 || (int)position.y != blkPos.y)
                                            {
                                                return;
                                            }
                                            dx = 1;
                                            break;
                                    }
                                    if(blk.type == MapBlock.Type.upStair)
                                    {
                                        string[] options = new string[2];
                                        options[0] = StringResource.goUp;
                                        options[1] = StringResource.cancel;
                                        //取消显示可移动的块
                                        DisabelAvailableArea();
                                        Vector3 dialogPos = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z + 3);
                                        UIManager.instance.CreateSelectDlg(Camera.main.WorldToScreenPoint(dialogPos), StringResource.upStair, options, (int idx) =>
                                        {
                                            if (idx == 0)
                                            {
                                                StartCoroutine(goUpStair((UpStair)blk, dx, dy));
                                                MapManager.instance.UpdateBlockState(position, sight, curLevel);
                                                return;
                                            }
                                            EnterIdle(false);
                                        });
                                    }
                                    if (blk.type == MapBlock.Type.downStair)
                                    {
                                        string[] options = new string[2];
                                        options[0] = StringResource.goDown;
                                        options[1] = StringResource.cancel;
                                        //取消显示可移动的块
                                        DisabelAvailableArea();
                                        Vector3 dialogPos = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z + 3);
                                        UIManager.instance.CreateSelectDlg(Camera.main.WorldToScreenPoint(dialogPos), StringResource.downStair, options, (int idx) =>
                                        {
                                            if (idx == 0)
                                            {
                                                StartCoroutine(goDownStair((DownStair)blk, dx, dy));
                                                MapManager.instance.UpdateBlockState(position, sight, curLevel);
                                                return;
                                            }
                                            EnterIdle(false);
                                        });
                                    }
                                }
                                //如果是可以移动的位置
                                else if (blk.available)
                                {
                                    focusBlk = blk;
                                    DisabelAvailableArea();
                                    StartCoroutine(goPath(walk.getPathTo(position, (Vector2)blkPos)));

                                }
                            }
                            
                        }
                    }
                }
            }

        }

        public void UseItem(int idx)
        {
            ItemPack imp = items[idx];
            if (imp != null)
            {
                imp.item.TakeEffect();
                imp.count--;
                if (imp.count <= 0)
                {
                    items[idx] = null;
                }
                ShowAvaliableArea();
            }
        }



        public void ToggleBag()
        {
            bool isActive = UIManager.instance.itemView.gameObject.activeSelf;
            if (isActive)
            {
                state = State.idle;
            }
            else
            {

                //打开背包
                state = State.waitUI;
            }
            UIManager.instance.itemView.SetActive(!isActive);
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
        
        public bool IsNearBy(Vector2 pos)
        {
            if ((int)pos.x == (int)position.x && Mathf.Abs(pos.y-position.y)<1.5f) {
                return true;
            }
            if((int)pos.y == (int)position.y&& Mathf.Abs(pos.x - position.x) < 1.5f){
                return true;
            }
            return false;
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

        public IEnumerator goUpStair(UpStair upStair,int dx,int dy)
        {
            state = State.animating;
            
            //关闭地图刷新
            Rect cameraRect = Camera.main.GetComponent<CameraMapWatch>().GetCameraRect();
            Camera.main.GetComponent<CameraMapWatch>().enabled = false;

            //关闭战争迷雾
            StartCoroutine(Camera.main.GetComponent<FX.FXFOW>().FadeOut(upDownFrame));

            //移动镜头
            UnityStandardAssets.Utility.SmoothFollow follow = Camera.main.GetComponent<UnityStandardAssets.Utility.SmoothFollow>();
            float oDistance = follow.distance;
            float oHeight = follow.height;
            float oPosDamping = follow.positionDamping;
            follow.distance = 2;
            follow.height = 2;
            follow.positionDamping = 10;
            for(int i = 0; i <= upDownFrame; i++)
            {
                yield return null;
            }
            

            //手动刷新下一层地图
            MapManager.instance.ShowCameraRect(cameraRect, curLevel + 1);
            MapManager.instance.UpdateBlockState(upStair.upPos, sight, curLevel + 1);

            //切换地图
            MapManager.instance.SwitchUp(upDownFrame);
            Map map = MapManager.instance.maps[curLevel];
            Vector2 mapPos = map.location;
            for (int i = 0; i < upDownFrame; i++)
            {
                float x = Mathf.Lerp(position.x, position.x + dx, i / (float)upDownFrame);
                float y = Mathf.Lerp(position.y, position.y + dy, i / (float)upDownFrame);
                float h = Mathf.Lerp(curLevel, curLevel + 1, i / (float)upDownFrame);
                transform.position = new Vector3(x + mapPos.x, h, y + mapPos.y);
                yield return null;
            }
            //切换curLevel和pos
            curLevel++;
            position = new Vector2(upStair.upPos.x + dx, upStair.upPos.y + dy);
            SetPosition();

            //开启战争迷雾
            StartCoroutine(Camera.main.GetComponent<FX.FXFOW>().FadeIn(upDownFrame));
            //移动镜头
            follow.distance = oDistance;
            follow.height = oHeight;
            for (int i = 0; i <= upDownFrame; i++)
            {
                yield return null;
            }
            follow.positionDamping = oPosDamping;

            //开启地图刷新
            Camera.main.GetComponent<CameraMapWatch>().enabled = true;

            //结束回合
            EndTurn();
            yield return null;
        }

        public IEnumerator goDownStair(DownStair downStair, int dx, int dy)
        {
            state = State.animating;

            //关闭地图刷新
            Rect cameraRect = Camera.main.GetComponent<CameraMapWatch>().GetCameraRect();
            Camera.main.GetComponent<CameraMapWatch>().enabled = false;

            //关闭战争迷雾
            StartCoroutine(Camera.main.GetComponent<FX.FXFOW>().FadeOut(upDownFrame));

            //移动镜头
            UnityStandardAssets.Utility.SmoothFollow follow = Camera.main.GetComponent<UnityStandardAssets.Utility.SmoothFollow>();
            float oDistance = follow.distance;
            float oHeight = follow.height;
            float oPosDamping = follow.positionDamping;
            follow.distance = 2;
            follow.height = 2;
            follow.positionDamping = 10;
            for (int i = 0; i <= upDownFrame; i++)
            {
                yield return null;
            }


            //手动刷新下一层地图
            MapManager.instance.ShowCameraRect(cameraRect, curLevel - 1);
            MapManager.instance.UpdateBlockState(downStair.lowPos, sight, curLevel - 1);

            //切换地图
            MapManager.instance.SwitchDown(upDownFrame);
            Map map = MapManager.instance.maps[curLevel];
            Vector2 mapPos = map.location;
            for (int i = 0; i < upDownFrame; i++)
            {
                float x = Mathf.Lerp(position.x, position.x + dx, i / (float)upDownFrame);
                float y = Mathf.Lerp(position.y, position.y + dy, i / (float)upDownFrame);
                float h = Mathf.Lerp(curLevel, curLevel -1, i / (float)upDownFrame);
                transform.position = new Vector3(x + mapPos.x, h, y + mapPos.y);
                yield return null;
            }
            //切换curLevel和pos
            curLevel--;
            position = new Vector2(downStair.lowPos.x + dx, downStair.lowPos.y + dy);
            SetPosition();

            //开启战争迷雾
            StartCoroutine(Camera.main.GetComponent<FX.FXFOW>().FadeIn(upDownFrame));
            //移动镜头
            follow.distance = oDistance;
            follow.height = oHeight;
            for (int i = 0; i <= upDownFrame; i++)
            {
                yield return null;
            }
            follow.positionDamping = oPosDamping;

            //开启地图刷新
            Camera.main.GetComponent<CameraMapWatch>().enabled = true;

            //结束回合
            EndTurn();
            yield return null;
        }

        public IEnumerator goPath(List<Vector2> path)
        {
            state = State.animating;
            Map map = MapManager.instance.maps[curLevel];
            Vector2 mapPos = map.location;
            //第一个path是自己所在的block
            for (int i = 1; i < path.Count; i++)
            {
                Vector2 next = path[i];
                for (int j = 0; j < moveFrame; j++)
                {
                    float x = Mathf.Lerp(position.x, next.x, j / (float)moveFrame);
                    float y = Mathf.Lerp(position.y, next.y, j / (float)moveFrame);
                    transform.position = new Vector3(x+mapPos.x, curLevel, y+mapPos.y);
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

        public IEnumerator goPathAndAction(List<Vector2> path, ActionCallback cbk)
        {
            state = State.animating;
            Map map = MapManager.instance.maps[curLevel];
            Vector2 mapPos = map.location;
            //第一个path是自己所在的block
            for (int i = 1; i < path.Count; i++)
            {
                Vector2 next = path[i];
                for (int j = 0; j < moveFrame; j++)
                {
                    float x = Mathf.Lerp(position.x, next.x, j / (float)moveFrame);
                    float y = Mathf.Lerp(position.y, next.y, j / (float)moveFrame);
                    transform.position = new Vector3(x + mapPos.x, curLevel, y + mapPos.y);
                    yield return null;
                }
                position = next;
                MapManager.instance.UpdateBlockState(position, sight, curLevel);
                availableStep--;
                yield return null;
            }
            cbk();
        }

        public void StartConversation(NpcCharactor npc)
        {
            state = State.animating;
            npc.OpenConversation(this);
        }

        public void EndConversation()
        {
            EnterIdle(false);
        }

    }
}