using UnityEngine;
using System.Collections;

//used to Manage the whole game
namespace MemoryTrap
{
    public enum GameState
    {
        //等待玩家输入
        waitPlayer,
        //等待AI行动
        waitAI,
        //等待ACTION战斗
        waitFight,
        //等待动画
        animating,
        //游戏失败
        fail
    }
    public class GameManager : MonoBehaviour
    {
        Battle curBattle;
        void TakeTurn()
        {

        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
