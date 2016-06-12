using UnityEngine;
using System.Collections;

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
        Battle curBattle;
        bool stateDone = false;
        public State curState = State.begin;
        public int turnCount = 0;
        /*需要实现的主要角色*/
        //public MainCharactor charactor;
        /*用来测试的charactor*/
        public Test.TestCharactor charactor;
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
            charactor.gameObject.SetActive(false);

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

            //初始化角色
            charactor.gameObject.SetActive(true);
            charactor.ResetPos();

            //关闭loading
            UIManager.instance.DisableLoading();

            while (curState != State.fail)
            {
                yield return TurnLoop();
                
                turnCount++;
            }
            yield return null;
        }


        IEnumerator TurnLoop()
        {
            while(curState!= State.turnOver)
            {
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
            /*框架*/
            charactor.BeginTurn();
            while(!charactor.turnOver){
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

        // Update is called once per frame
        void Update()
        {

        }
    }
}
