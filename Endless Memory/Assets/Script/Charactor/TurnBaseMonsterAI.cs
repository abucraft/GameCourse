using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MemoryTrap
{
    public abstract class TurnBaseMonsterAI : MonoBehaviour
    {
        // Use this for initialization
        //void Start () {

        //}

        // Update is called once per frame
        //void Update () {

        //}


        
        public abstract List<Vector2> behavior(EnemyCharactor self, TurnBaseCharactor main, List<EnemyCharactor> monsters);
        public abstract TurnBaseMonsterAI behaviorChange(EnemyCharactor self, TurnBaseCharactor main, List<EnemyCharactor> monsters);

    }

    public class EmptyAI : TurnBaseMonsterAI
    {
        override public List<Vector2> behavior(EnemyCharactor self, TurnBaseCharactor main, List<EnemyCharactor> monsters)
        {
            return new List<Vector2>();
        }

        override public TurnBaseMonsterAI behaviorChange(EnemyCharactor self, TurnBaseCharactor main, List<EnemyCharactor> monsters)
        {
            if (self.inBattle) return this;
            int vagueDistanceMain = (int)(System.Math.Abs(self.postion.x - main.postion.x)) + (int)(System.Math.Abs(self.postion.y - main.postion.y));
            if (vagueDistanceMain > main.step + self.step + 1) return this;
            

            List<Vector2> pathToMain = self.walk.getPathTo(self.postion, main.postion);
            if (pathToMain.Count == 0) return this;
            if (pathToMain.Count < self.step) return new ForwardMainAI();

            bool battled;
            Battle curBattle = GameObject.Find("GameManager").GetComponent<GameManager>().curBattle;
            if (curBattle == null) battled = false;
            else battled = true;

            if (pathToMain.Count > main.step + self.step + 1)
            {
                
                return this;
            }
            if(pathToMain.Count>self.step)
            {

            }

            return new ForwardMainAI();
        }
    }

    public class ForwardMainAI : TurnBaseMonsterAI
    {
        List<Vector2> forwardPath = null;

        override public List<Vector2> behavior(EnemyCharactor self, TurnBaseCharactor main, List<EnemyCharactor> monsters)
        {
            if (forwardPath == null)
                forwardPath = self.walk.getPathTo(self.postion, main.postion);
            List<Vector2> path = new List<Vector2>();
            List<Vector2> newForwardPath = new List<Vector2>();
            for (int i = 0; i < forwardPath.Count; i++)
            {
                if (i < self.step + 1)
                    path.Add(forwardPath[i]);
                else
                    newForwardPath.Add(forwardPath[i]);
            }
            forwardPath = newForwardPath;
            return path;
        }

        override public TurnBaseMonsterAI behaviorChange(EnemyCharactor self, TurnBaseCharactor main, List<EnemyCharactor> monsters)
        {
            if (self.inBattle) return new EmptyAI();
            return this;
        }
    }

    public class ForwardCompaionAI : TurnBaseMonsterAI
    {
        EnemyCharactor compaion;
        List<Vector2> forwardPath = null;

        ForwardCompaionAI(EnemyCharactor _compaion)
        {
            compaion = _compaion;
        }

        override public List<Vector2> behavior(EnemyCharactor self, TurnBaseCharactor main, List<EnemyCharactor> monsters)
        {
            if(forwardPath == null)
                forwardPath = self.walk.getPathTo(self.postion, compaion.postion);
            List<Vector2> path = new List<Vector2>();
            List<Vector2> newForwardPath = new List<Vector2>();
            for (int i = 0;i<forwardPath.Count;i++)
            {
                if (i < self.step + 1)
                    path.Add(forwardPath[i]);
                else
                    newForwardPath.Add(forwardPath[i]);
            }
            forwardPath = newForwardPath;
            return path;
        }

        override public TurnBaseMonsterAI behaviorChange(EnemyCharactor self, TurnBaseCharactor main, List<EnemyCharactor> monsters)
        {
            bool[,] occupied = self.walk.getOccupied();
            for(int i=-1;i<2;i++)
                for(int j=-1;j<2;j++)
                {
                    if (i == 0 && j == 0) continue;
                    if (occupied[(int)self.postion.x + i, (int)self.postion.y + j])
                    {
                        return new EmptyAI();
                    }
                }

            forwardPath = self.walk.getPathTo(self.postion, compaion.postion);
            if (forwardPath.Count == 2) return new EmptyAI().behaviorChange(self, main, monsters); 
            if (forwardPath.Count <= self.step + 2) return this;

            int vagueDistanceMain = (int)(System.Math.Abs(self.postion.x - main.postion.x)) + (int)(System.Math.Abs(self.postion.y - main.postion.y));
            if (vagueDistanceMain <= self.step + 1)
            {
                List<Vector2> pathToMain = self.walk.getPathTo(self.postion, main.postion);
                if (pathToMain.Count <= self.step + 2)
                    return new ForwardMainAI().behaviorChange(self, main, monsters);
            }

            return this;
        }
    }

}
