using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MemoryTrap
{
    public class TurnBaseCharactor : MonoBehaviour
    {
        public Vector2 postion;
        public int hp;
        public int step;
        public TurnBaseWalk walk;
        public bool turnOver = false;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    public class MainCharactor : TurnBaseCharactor
    {

    }

    public class EnemyCharactor : TurnBaseCharactor
    {
        public TurnBaseMonsterAI ai = new EmptyAI();
        public bool inBattle = false;
    }
}
