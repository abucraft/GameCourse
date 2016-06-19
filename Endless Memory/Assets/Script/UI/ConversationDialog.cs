using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
namespace MemoryTrap
{
    namespace UI
    {
        public class ConversationDialog : MonoBehaviour
        {
            public Text discription;
            public Button btn;
            public Canvas btnCanvas;
            public delegate void CallBack(int idx);
            public CallBack m_cbk;
            public void Init(string description, string[] options, CallBack cbk)
            {
                m_cbk = cbk;
                discription.text = description;
                Vector3 lastPos = btn.GetComponent<RectTransform>().localPosition;
                if (options != null)
                {
                    for (int i = 0; i < options.Length; i++)
                    {
                        GameObject nbtnObj = Instantiate<GameObject>(btn.gameObject);
                        RectTransform rct = nbtnObj.GetComponent<RectTransform>();
                        rct.SetParent(btnCanvas.GetComponent<RectTransform>(), false);
                        rct.localPosition = lastPos;
                        rct.GetComponentInChildren<Text>().text = options[i];
                        Button nbtn = nbtnObj.GetComponent<Button>();
                        AddListener(nbtn, i);
                        lastPos.y -= rct.rect.height;
                    }
                }
            }

            void AddListener(Button b, int idx)
            {
                b.onClick.AddListener(() =>
                {
                    m_cbk(idx);
                    //delete parent
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