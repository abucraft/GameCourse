using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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
        public enum AIState
        {
            begin,
            empty,
            forwardMain,
            forwardCompaion,
            defense
        }

        public abstract AIState nowbehavior();
        public abstract List<Vector2> behavior(EnemyCharactor self, TurnBaseCharactor main, List<EnemyCharactor> monsters);
        public abstract TurnBaseMonsterAI behaviorChange(EnemyCharactor self, TurnBaseCharactor main, List<EnemyCharactor> monsters);

    }

    public class BeginAI : TurnBaseMonsterAI
    {
        public override AIState nowbehavior()
        {
            return AIState.begin;
        }

        public override List<Vector2> behavior(EnemyCharactor self, TurnBaseCharactor main, List<EnemyCharactor> monsters)
        {
            throw new NotImplementedException();
        }

        public override TurnBaseMonsterAI behaviorChange(EnemyCharactor self, TurnBaseCharactor main, List<EnemyCharactor> monsters)
        {
            // 检查周围是否有door存在
            Map map = self.walk.getMap();
            int x = (int)self.position.x;
            int y = (int)self.position.y;
            int[,] array = { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 } };
            for (int i = 0; i < 4; i++)
            {
                int xNew = x + array[i,0];
                int yNew = y + array[i,1];
                if (xNew > -1 && xNew < map.map.GetLength(0)
                    && yNew > -1 && yNew < map.map.GetLength(1))
                    if (map.map[xNew, yNew].type == MapBlock.Type.door)
                        return new DefenseAI();
            }

            // 检查是否是关隘
            if ((!self.walk.reachableMapBlock(new Vector2(x + 1, y), map) && !self.walk.reachableMapBlock(new Vector2(x - 1, y), map))
                    || (!self.walk.reachableMapBlock(new Vector2(x, y + 1), map) && !self.walk.reachableMapBlock(new Vector2(x, y - 1), map)))
                return new DefenseAI();

            // 检查附近是否有同伴
            Dictionary<Vector2, TurnBaseCharactor> locationCharactor = GameManager.instance.levelLocationCharactors[self.curLevel];
            self.walk.getReachableArea(self.position, self.step);
            if (self.walk.reachableCharator.Count != 0)
            {
                if(!self.walk.reachableCharator[0].Equals(main))
                    return new ForwardCompaionAI((EnemyCharactor)locationCharactor[self.walk.reachableCharator[0]]);
                else if(self.walk.reachableCharator.Count != 1)
                    return new ForwardCompaionAI((EnemyCharactor)locationCharactor[self.walk.reachableCharator[1]]);
            }

            return new EmptyAI().behaviorChange(self,main,monsters);
        }
    }

    public class EmptyAI : TurnBaseMonsterAI
    {
        public override AIState nowbehavior()
        {
            return AIState.empty;
        }

        override public List<Vector2> behavior(EnemyCharactor self, TurnBaseCharactor main, List<EnemyCharactor> monsters)
        {
            return new List<Vector2>();
        }

        override public TurnBaseMonsterAI behaviorChange(EnemyCharactor self, TurnBaseCharactor main, List<EnemyCharactor> monsters)
        {
            if (self.inBattle) return this;
            int vagueDistanceMain = (int)(System.Math.Abs(self.position.x - main.position.x)) + (int)(System.Math.Abs(self.position.y - main.position.y));
            if (vagueDistanceMain > main.step + self.step + 1) return this;
            

            List<Vector2> pathToMain = self.walk.getPathTo(self.position, main.position);
            // 无法到达主角所在处，不动
            if (pathToMain.Count == 0) return this;
            // 可以直接到达主角所在处，变为ForwardMainAI
            if (pathToMain.Count <= self.step+2) return new ForwardMainAI();

            // 检查是否有战斗，有战斗，且可以两回合内进入战斗，则变为ForwardMainAI
            bool battled = false;
            Battle curBattle = GameManager.instance.curBattle;
            if (curBattle != null) battled = true;

            if(battled)
            {
                if (pathToMain.Count <= 2 * self.step + 2) return new ForwardMainAI();
            }

            return this;
        }
    }

    public class DefenseAI : EmptyAI
    {
        public override AIState nowbehavior()
        {
            return AIState.defense;
        }

        override public TurnBaseMonsterAI behaviorChange(EnemyCharactor self, TurnBaseCharactor main, List<EnemyCharactor> monsters)
        {
            return this;
        }
    }

    public class ForwardMainAI : TurnBaseMonsterAI
    {
        List<Vector2> forwardPath = null;

        public override AIState nowbehavior()
        {
            return AIState.forwardMain;
        }

        override public List<Vector2> behavior(EnemyCharactor self, TurnBaseCharactor main, List<EnemyCharactor> monsters)
        {
            forwardPath = self.walk.getPathTo(self.position, main.position);
            List<Vector2> path = new List<Vector2>();
 
            for (int i = 0; i < forwardPath.Count; i++)
            {
                if (i < self.step + 1)
                    path.Add(forwardPath[i]);
            }
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

        public override AIState nowbehavior()
        {
            return AIState.forwardCompaion;
        }

        public ForwardCompaionAI(EnemyCharactor _compaion)
        {
            compaion = _compaion;
        }

        override public List<Vector2> behavior(EnemyCharactor self, TurnBaseCharactor main, List<EnemyCharactor> monsters)
        {
            if(forwardPath == null)
                forwardPath = self.walk.getPathTo(self.position, compaion.position);
            List<Vector2> path = new List<Vector2>();
            
            for (int i = 0;i<forwardPath.Count;i++)
            {
                if (i < self.step + 1)
                    path.Add(forwardPath[i]);
            }
            return path;
        }

        override public TurnBaseMonsterAI behaviorChange(EnemyCharactor self, TurnBaseCharactor main, List<EnemyCharactor> monsters)
        {
            if (self.inBattle) return new EmptyAI();

            Dictionary<Vector2, TurnBaseCharactor> locationCharactors = GameManager.instance.levelLocationCharactors[self.curLevel];
            // 检视四周是否已经有charactor，有的话
            // 如果charactor已经停止行动，变为EmptyAI
            // 如果charactor未停止行动
            for(int i=-1;i<2;i++)
                for(int j=-1;j<2;j++)
                {
                    if (i != 0 && j != 0) continue;
                    if (i == 0 && j == 0) continue;
                    
                    Vector2 tmp = new Vector2((int)self.position.x + i, (int)self.position.y + j);
                    
                    if (locationCharactors.ContainsKey(tmp))
                        return new EmptyAI();
                }

            if(compaion.ai.nowbehavior() == AIState.forwardMain)
            {
                return new ForwardMainAI();
            }

            // 检查和Compaion的距离，如果可以马上到达的话，那么还是FowardCompaionAI
            forwardPath = self.walk.getPathTo(self.position, compaion.position);
            if (forwardPath.Count <= self.step + 2 && compaion.turnOver == true)
                return this;

            // 检查和MainCharactor的距离，如果可以直接到达，那么就变成ForwardMainAI
            int vagueDistanceMain = (int)(System.Math.Abs(self.position.x - main.position.x)) + (int)(System.Math.Abs(self.position.y - main.position.y));
            if (vagueDistanceMain <= self.step + 1)
            {
                List<Vector2> pathToMain = self.walk.getPathTo(self.position, main.position);
                if (pathToMain.Count <= self.step + 2 && pathToMain.Count != 0)
                    return new ForwardMainAI();
            }

            return this;
        }
    }

}
