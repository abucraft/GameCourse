using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MemoryTrap
{
    public class TurnBaseCharactor : MonoBehaviour
    {
        public Vector2 position;
        public int curLevel = 0;
        [SerializeField]
        protected int _hp;
        [SerializeField]
        protected int _curHp;
        public virtual int hp { get { return _hp; } set { _hp = value; } }
        public virtual int curHp { get { return _curHp; } set { _hp = value; } }

        public int step;
        public TurnBaseWalk _walk;
        public virtual TurnBaseWalk walk
        {
            get
            {
                if (_walk == null)
                {
                    _walk = new TurnMainWalk();
                }
                return _walk;
            }
        }
        public bool turnOver = true;
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

    

    
}
