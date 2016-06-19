using UnityEngine;
using System.Collections.Generic;
using System.Collections;
namespace MemoryTrap
{
    public class NpcCharactor : TurnBaseCharactor
    {
        public enum State
        {
            waiting,
            talked
        }
        public State state = State.waiting;

        //每隔10帧刷新自身状态
        public int totalFreshCount = 10;
        private int curFreshCount;

        public string[] firstTalk;
        public string[] secondTalk;


        public void OpenConversation(MainCharactor main)
        {
            if(state == State.waiting)
            {
                StartCoroutine(ShowFirstTalk(main));
            }
            else
            {
                StartCoroutine(ShowSecondTalk(main));
            }
        }

        public IEnumerator ShowFirstTalk(MainCharactor main)
        {
            Vector3 dialogPos = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z + 3);
            UI.ConversationDialog dlg = UIManager.instance.CreateConversationDlg(Camera.main.WorldToScreenPoint(dialogPos), "".ToString(), null, null);
            for (int i = 0; i < firstTalk.Length; i++)
            {
                string word = firstTalk[i];
                
                
                for(int j = 1; j < word.Length; j++)
                {
                    dlg.discription.text = word.Substring(0, j);
                    if (Input.GetMouseButtonUp(0))
                    {
                        yield return null;
                        break;
                    }
                    yield return null;
                }
                dlg.discription.text = word;
                while (!Input.GetMouseButtonUp(0))
                {
                    yield return null;
                }
                yield return null;
                
            }
            state = State.talked;
            main.EndConversation();

            Destroy(dlg.transform.parent.gameObject);
            yield return null;

        }

        public IEnumerator ShowSecondTalk(MainCharactor main)
        {
            int idx = Random.Range(0, secondTalk.Length);
            string word = secondTalk[idx];
            Vector3 dialogPos = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z + 3);
            UI.ConversationDialog dlg = UIManager.instance.CreateConversationDlg(Camera.main.WorldToScreenPoint(dialogPos), word[0].ToString(), null, null);
            for (int j = 1; j < word.Length; j++)
            {
                dlg.discription.text = word.Substring(0, j);
                if (Input.GetMouseButtonDown(0))
                {
                    break;
                }
                yield return null;
            }
            dlg.discription.text = word;
            while (!Input.GetMouseButtonDown(0))
            {
                yield return null;
            }
            Destroy(dlg.transform.parent.gameObject);
            state = State.talked;
            main.EndConversation();
            yield return null;
        }


        public override TurnBaseWalk walk
        {
            get
            {
                if (_walk == null)
                {
                    _walk = new TurnEnemyWalk();
                }
                return _walk;
            }
        }

        public override void BeginTurn()
        {
            base.BeginTurn();
        }

        void Start()
        {
            curFreshCount = (int)Random.Range(0, totalFreshCount);
            turnOver = true;
        }

        void Update()
        {
            if (!turnOver)
            {
                
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
            Collider cd = GetComponent<Collider>();
            cd.enabled = true ;
            mesh.enabled = true;
        }

        public void HideMesh()
        {
            //Debug.Log("hide mesh");
            Renderer mesh = GetComponent<Renderer>();
            Collider cd = GetComponent<Collider>();
            cd.enabled = false;
            mesh.enabled = false;
        }
        

    }
}