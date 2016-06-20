using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MemoryTrap
{
    namespace UI
    {
        public class InfomationDialog : MonoBehaviour
        {
            public Text nameTex;
            public Text descriptionTex;
            public Text healthTex;
            public Text attackTex;
            public RectTransform totalLife;
            public RectTransform restLife;
            public Button btn;
            public Canvas btnCanvas;
            public delegate void CallBack(int idx);
            public CallBack m_cbk;
            public void Init(EnemyCharactor enemy, string[] options, CallBack cbk)
            {
                m_cbk = cbk;
                nameTex.text = enemy.myname;
                descriptionTex.text = enemy.description;
                float totalWidth = totalLife.sizeDelta.x;
                float restWidth = enemy.curHp / (float)enemy.hp * totalLife.sizeDelta.x;
                Vector2 restSizeDelta = restLife.sizeDelta;
                restSizeDelta.x = restWidth;
                restLife.sizeDelta = restSizeDelta;
                healthTex.text = enemy.curHp.ToString() + '/' + enemy.hp.ToString();
                attackTex.text = enemy.attack.ToString();
                Debug.Assert(options.Length >= 1);
                btn.GetComponentInChildren<Text>().text = options[0];
                AddListener(btn, 0);
                Vector3 lastPos = btn.GetComponent<RectTransform>().localPosition;
                for (int i = 1; i < options.Length; i++)
                {
                    GameObject nbtnObj = Instantiate<GameObject>(btn.gameObject);
                    RectTransform rct = nbtnObj.GetComponent<RectTransform>();
                    rct.parent = btnCanvas.GetComponent<RectTransform>();
                    lastPos.y -= rct.rect.height;
                    rct.localPosition = lastPos;
                    rct.GetComponentInChildren<Text>().text = options[i];
                    Button nbtn = nbtnObj.GetComponent<Button>();
                    AddListener(nbtn, i);
                }

            }

            void AddListener(Button b, int idx)
            {
                b.onClick.AddListener(() =>
                {
                    m_cbk(idx);
                    //item dlg 需要阻挡其他ui因此挂在一个canvas之下
                    Destroy(gameObject.transform.parent.gameObject);
                });
            }

            void Update()
            {
                GetComponent<AdaptiveByChildren>().SetLayoutHorizontal();
                GetComponent<AdaptiveByChildren>().SetLayoutVertical();
            }
        }
    }
}