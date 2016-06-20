using UnityEngine;
using System.Collections.Generic;
using System.Collections;
namespace MemoryTrap
{
    public class EnemyCharactor : TurnBaseCharactor
    {
        public enum State
        {
            idle,
            animating
        }
        public TurnBaseMonsterAI ai;
        public bool inBattle = false;
        public State state = State.idle;

        public string myname;
        public string description;
        public int attack;
        //每隔10帧刷新自身状态
        public int totalFreshCount = 10;
        private int curFreshCount;


        public override TurnBaseWalk walk
        {
            get
            {
                if(_walk == null)
                {
                    _walk = new TurnEnemyWalk();
                }
                return _walk;
            }
        }

        public override void BeginTurn()
        {
            base.BeginTurn();
            state = State.idle;
        }

        void OnEnable()
        {
            DisableRigid();
        }

        void OnDisable()
        {
            EnableRigid();
        }

        void Start()
        {
            curFreshCount = (int)Random.Range(0, totalFreshCount);
            turnOver = true;
            ai = new EmptyAI();
        }

        void Update()
        {
            if (!turnOver)
            {
                //如果是idle状态，采取行动
                if(state == State.idle)
                {
                    ai = ai.behaviorChange(this, GameManager.instance.mainCharactor, null);
                    List<Vector2> behavePath = ai.behavior(this, GameManager.instance.mainCharactor, null);
                    //Debug.Log(behavePath.Count);
                    StartCoroutine(goPath(behavePath));
                }
            }
            //隔帧刷新
            if (curFreshCount > totalFreshCount)
            {
                Map map = MapManager.instance.maps[curLevel];
                if (map.map[(int)position.x, (int)position.y].inSight)
                {
                    ShowMesh();
                }
                else
                {
                    HideMesh();
                }
            }
            curFreshCount++;
        }

        public void SetPosition(Vector2 pos)
        {
            Map map = MapManager.instance.maps[curLevel];
            position = pos;
            Vector2 mapPos = map.location;
            transform.position = new Vector3(pos.x + mapPos.x, curLevel, pos.y + mapPos.y);
        }

        public void ShowMesh()
        {
            Renderer mesh = GetComponent<Renderer>();
            Animator anime = GetComponent<Animator>();
            Rigidbody rg = GetComponent<Rigidbody>();
            if(mesh != null)
                mesh.enabled = true;
            if (anime != null)
                anime.enabled = true;
            Renderer[] childRenders = GetComponentsInChildren<Renderer>();
            for(int i = 0; i < childRenders.Length; i++)
            {
                childRenders[i].enabled = true;
            }
        }

        public void DisableRigid()
        {
            if(GetComponent<Rigidbody>()!=null)
                GetComponent<Rigidbody>().isKinematic = true;
        }

        public void EnableRigid()
        {
            if(GetComponent<Rigidbody>()!=null)
                GetComponent<Rigidbody>().isKinematic = false;
        }

        public void HideMesh()
        {
            //Debug.Log("hide mesh");
            Renderer mesh = GetComponent<Renderer>();
            Animator anime = GetComponent<Animator>();
            if(mesh!=null)
                mesh.enabled = false;
            if (anime != null)
                anime.enabled = false;
            Renderer[] childRenders = GetComponentsInChildren<Renderer>();
            for (int i = 0; i < childRenders.Length; i++)
            {
                childRenders[i].enabled = false;
            }
        }

        IEnumerator goPath(List<Vector2> path)
        {
            state = State.animating;
            Map map = MapManager.instance.maps[curLevel];
            Vector2 mapPos = map.location;
            Dictionary<Vector2, EnemyCharactor> curLevelCharactor = GameManager.instance.levelEnemyCharactors[curLevel];
            //将Animator设置为移动
            Animator animator = GetComponent<Animator>();
            Animation animation = GetComponent<Animation>();
            if (animator != null)
            {
                animator.SetBool("Walk", true);
            }
            if (animation != null)
            {
                animation.Play("Walk");
            }
            //第一个path是自己所在的block
            for (int i = 1; i < path.Count; i++)
            {
                Vector2 next = path[i];
                int dx = (int)(next.x - position.x);
                int dy = (int)(next.y - position.y) < 0 ? (int)(next.y - position.y) : 1;
                transform.rotation = Quaternion.Euler(new Vector3(0, dx * 90 + (dy-1)*90, 0));

                for (int j = 0; j < moveFrame; j++)
                {
                    float x = Mathf.Lerp(position.x, next.x, j / (float)moveFrame);
                    float y = Mathf.Lerp(position.y, next.y, j / (float)moveFrame);
                    transform.position = new Vector3(x + mapPos.x, curLevel, y + mapPos.y);
                    
                    yield return null;
                }
                curLevelCharactor.Remove(position);
                position = next;
                curLevelCharactor.Add(position, this);
                yield return null;
            }
            if (animator != null)
            {
                animator.SetBool("Walk", false);
            }
            if (animation != null)
            {
                animation.Play("Wait");
            }
            turnOver = true;
            yield return null;
        }
        
    }
}