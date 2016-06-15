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
        public int turnCount = 0;
        /*主要角色*/
        public MainCharactor mainCharactor;
        /*需要实现的怪物*/
        //public List<EnemyCharactor> enemyCharactors;
        //不同层之间的怪物位置
        public Dictionary<int, Dictionary<Vector2, TurnBaseCharactor>> levelLocationCharactors;
        //public Dictionary<Vector2, TurnBaseCharactor> locationCharactors;
        /*用来测试的charactor*/
        //public Test.TestCharactor charactor;
        public Camera fowCamera;
        public static GameManager instance;

        void Start()
        {
            if(instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
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
            //生成敌人
            CreateEnemy();
            //开启FOW效果
            fowCamera.gameObject.SetActive(true);
            Camera.main.GetComponent<FX.FXFOW>().enabled = true;
            Camera.main.GetComponent<CameraMapWatch>().RefreshCameraRect();

            //初始化角色
            mainCharactor.gameObject.SetActive(true);
            mainCharactor.ResetPos();

            //关闭loading
            UIManager.instance.DisableLoading();
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
            Debug.Log("turn loop");
            while(curState!= State.turnOver)
            {
                Debug.Log("loop state:" + curState);
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
            yield return null;
        }

        IEnumerator WaitPlayer()
        {
            Debug.Log("wait player");
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

        public void CreateEnemy()
        {
            
            //todo:应急措施
            levelLocationCharactors = new Dictionary<int, Dictionary<Vector2, TurnBaseCharactor>>();
            for(int i = 0; i < MapManager.instance.maps.Length; i++)
            {
                levelLocationCharactors[i] = new Dictionary<Vector2, TurnBaseCharactor>();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
