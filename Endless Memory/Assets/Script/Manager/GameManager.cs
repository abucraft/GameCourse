using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//used to Manage the whole game
namespace MemoryTrap
{
    
    public class GameManager : MonoBehaviour
    {
        public enum State
        {
            //游戏开始
            begin,
            //等待玩家输入
            waitPlayer,
            //等待AI行动
            waitAI,
            //等待ACTION战斗
            waitFight,
            //Turn Over
            turnOver,
            //游戏失败
            fail
        }
        public Battle curBattle;
        bool stateDone = false;
        public State curState = State.begin;
        public int curLevel = 0;
        public int turnCount = 0;
        public int minEnemyPRoom = 5;
        public int maxEnemyPRoom = 10;
        /*主要角色*/
        public MainCharactor mainCharactor;
        /*需要实现的怪物*/
        public List<EnemyCharactor> enemyCharactors = new List<EnemyCharactor>();
        //不同层之间的怪物位置
        public Dictionary<int, Dictionary<Vector2, EnemyCharactor>> levelEnemyCharactors;

        //物品位置
        public Dictionary<int, Dictionary<Vector2, ItemPack>> levelItemPacks;

        //npc位置
        public Dictionary<int, Dictionary<Vector2, NpcCharactor>> levelNPCCharactors;
        //public Dictionary<Vector2, TurnBaseCharactor> locationCharactors;
        /*用来测试的charactor*/
        //public Test.TestCharactor charactor;
        public Camera fowCamera;
        public static GameManager instance;

        public EnemyCharactor enemy1;
        public NpcCharactor npc1;

        public System.Random rand;
        void Start()
        {
            if(instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            //初始化随机种子
            rand = new System.Random(System.DateTime.Now.Millisecond);
            StartCoroutine(GameLoop());
        }

        IEnumerator GameLoop()
        {
            //等待游戏载入
            //关闭FOW效果
            fowCamera.gameObject.SetActive(false);
            Camera.main.GetComponent<FX.FXFOW>().enabled = false;
            //TestCharactor disable
            mainCharactor.gameObject.SetActive(false);

            //显示loading
            UIManager.instance.ShowLoading();

            //等待MapManager生成或加载完毕
            while(MapManager.instance.state!= MapManager.State.ready)
            {
                yield return null;
            }
            
            //开启FOW效果
            fowCamera.gameObject.SetActive(true);
            Camera.main.GetComponent<FX.FXFOW>().enabled = true;
            Camera.main.GetComponent<CameraMapWatch>().RefreshCameraRect();

            //生成道具dictionary
            CreateItemDictionary();

            //初始化角色
            mainCharactor.gameObject.SetActive(false);
            mainCharactor.ResetPos();


            //生成敌人
            yield return CreateEnemy();

            //生成道具
            yield return CreateItems();

            PlaceNPC();
            mainCharactor.gameObject.SetActive(true);

            //刷新可见物品
            OnBlockSightChange(curLevel);

            //关闭loading
            UIManager.instance.DisableLoading();
            //开始游戏循环
            curState = State.waitPlayer;
            while (curState != State.fail)
            {
                yield return TurnLoop();
                
                turnCount++;
                curState = State.waitPlayer;
            }
            yield return null;
        }


        IEnumerator TurnLoop()
        {
            //Debug.Log("turn loop");
            while(curState!= State.turnOver)
            {
                //Debug.Log("loop state:" + curState);
                //玩家控制结束以后是AI
                if(curState == State.waitPlayer)
                {
                    yield return WaitPlayer();
                    curState = State.waitAI;
                }
                //AI结束以后是即时战斗
                if(curState == State.waitAI)
                {
                    yield return WaitAI();
                    curState = State.waitFight;
                }
                //即时战斗等待完毕表示此回合结束
                if(curState == State.waitFight)
                {
                    yield return WaitFight();
                    curState = State.turnOver;
                }
                yield return null;
            }
        }
        IEnumerator WaitAI()
        {
            foreach(EnemyCharactor enemy in enemyCharactors)
            {
                if (enemy.gameObject.activeSelf)
                {
                    enemy.BeginTurn();
                    while (!enemy.turnOver)
                    {
                        yield return null;
                    }
                }
            }
            yield return null;
        }

        IEnumerator WaitPlayer()
        {
            //Debug.Log("wait player");
            mainCharactor.BeginTurn();
            while(!mainCharactor.turnOver){
                yield return null;
            }
            
            yield return null;
        }

        IEnumerator WaitFight()
        {
            yield return null;
        }

        void CheckState()
        {
            if(curState == State.turnOver)
            {
                curState = State.waitPlayer;
            }
        }

        public IEnumerator CreateEnemy()
        {
            
            //todo:应急措施
            levelEnemyCharactors = new Dictionary<int, Dictionary<Vector2, EnemyCharactor>>();
            for(int i = 0; i < MapManager.instance.maps.Length; i++)
            {
                levelEnemyCharactors[i] = new Dictionary<Vector2, EnemyCharactor>();
            }
            yield return null;
            //每个房间放置几个
            for(int i = 0; i < MapManager.instance.maps.Length; i++)
            {
                Map map = MapManager.instance.maps[i];
                List<RectI> rooms = map.roomList;
                foreach(RectI room in rooms)
                {
                    List<Vector2I> floorList = new List<Vector2I>();
                    for(int x = room.left; x <= room.right; x++)
                    {
                        for(int y = room.top; y <= room.bottom; y++)
                        {
                            if(map.map[x,y].type == MapBlock.Type.floor)
                                floorList.Add(new Vector2I(x, y));
                        }
                    }
                    int enemyCount = rand.Next(minEnemyPRoom, maxEnemyPRoom);
                    for(int n = 0; n < enemyCount; n++)
                    {
                        Vector2I floor = floorList[rand.Next(0, floorList.Count)];
                        if(floor.x!= (int)mainCharactor.position.x || floor.y != (int)mainCharactor.position.y)
                        {
                            GameObject eobj = Instantiate<GameObject>(enemy1.gameObject);
                            eobj.GetComponent<EnemyCharactor>().curLevel = i;
                            eobj.GetComponent<EnemyCharactor>().SetPosition((Vector2)floor);
                            eobj.transform.parent = transform;
                            levelEnemyCharactors[i][(Vector2)floor] = eobj.GetComponent<EnemyCharactor>();
                            enemyCharactors.Add(eobj.GetComponent<EnemyCharactor>());
                            if(i != 0)
                            {
                                eobj.SetActive(false);
                            }
                        }
                    }
                    yield return null;
                }
            }
        }

        public void CreateItemDictionary()
        {
            levelItemPacks = new Dictionary<int, Dictionary<Vector2, ItemPack>>();
            for (int i = 0; i < MapManager.instance.maps.Length; i++)
            {
                levelItemPacks[i] = new Dictionary<Vector2, ItemPack>();
            }
        }

        public IEnumerator CreateItems()
        {
            Vector2 chPos = mainCharactor.position;
            Vector2 firstItmPos = chPos;
            firstItmPos.x--;
            Dictionary<Vector2, ItemPack> packs = levelItemPacks[curLevel];
            ItemPack firstItm = new ItemPack();
            firstItm.item = new HPBottle();
            firstItm.count = 2;
            packs[firstItmPos] = firstItm;
            yield return null;
        }

        public void PlaceNPC()
        {
            levelNPCCharactors = new Dictionary<int, Dictionary<Vector2, NpcCharactor>>();
            for(int i = 0; i < MapManager.instance.maps.Length; i++)
            {
                levelNPCCharactors[i] = new Dictionary<Vector2, NpcCharactor>();
            }
            npc1.curLevel = 0;
            Vector2 mainPos = mainCharactor.position;
            Vector2I npcPos1 = new Vector2I((int)mainPos.x, (int)mainPos.y + 1);
            npc1.SetPosition((Vector2)npcPos1);
            levelNPCCharactors[0][(Vector2)npcPos1] = npc1;
            npc1.gameObject.SetActive(true);
        }

        public void OnBlockSightChange(int level)
        {
            Update();
            Map map = MapManager.instance.maps[curLevel];
            RectI area = map.curArea;
            Dictionary<Vector2, ItemPack> curPacks = levelItemPacks[curLevel];
            for(int x = area.left; x <= area.right; x++)
            {
                for(int y = area.top; y <= area.bottom; y++)
                {
                    if (map.map[x, y].inSight)
                    {
                        Vector2 pos = new Vector2(x, y);
                        if (curPacks.ContainsKey(pos))
                        {
                            curPacks[pos].item.inSight = true;
                            curPacks[pos].item.gameObject.GetComponent<ItemHolder>().SetPos(new Vector2I(x, y), curLevel);
                        }
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(curLevel != mainCharactor.curLevel)
            {
                //唤醒所有这一层的怪物
                //disable上一层的怪物
                foreach(EnemyCharactor enemy in levelEnemyCharactors[curLevel].Values)
                {
                    enemy.gameObject.SetActive(false);
                }
                foreach(EnemyCharactor enemy in levelEnemyCharactors[mainCharactor.curLevel].Values)
                {
                    enemy.gameObject.SetActive(true);
                }
                curLevel = mainCharactor.curLevel;
            }
        }
    }
}
