using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MemoryTrap
{
    public class TurnBaseCharactor : MonoBehaviour
    {
        public Vector2 position;
        public int curLevel = 0;
        public int hp;
        public int step;
        public TurnBaseWalk walk = new TurnMainWalk();
        public bool turnOver = false;
        [HideInInspector]
        public int moveFrame = 10;
        public virtual void BeginTurn()
        {
            turnOver = false;
            
        }

        // Use this for initialization
        void Start()
        {
            walk.curLevel = curLevel;
        }

        // Update is called once per frame
        void Update()
        {

        }
        
    }

    

    public class EnemyCharactor : TurnBaseCharactor
    {
        public TurnBaseMonsterAI ai = new EmptyAI();
        public bool inBattle = false;
    }
}
