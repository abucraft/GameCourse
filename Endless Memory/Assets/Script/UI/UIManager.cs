using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace MemoryTrap
{
    public class UIManager : MonoBehaviour
    {
        public GameObject loading;
        public GameObject objectCanvas;
        //public GameObject blockCanvas;
        public UI.SelectDialog selectDlg;
        public UI.ItemDialog itemDlg;
        public UI.ConversationDialog convDlgL;
        public UI.ConversationDialog convDlgR;
        public GameObject itemView;
        public GameObject bagBtn;
        public GameObject turnOverBtn;

        public UI.HealthBar hpBar;

        public static UIManager instance;
       
        // Use this for initialization
        void Start()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            //CreateConversationDlg(new Vector3(Screen.width / 2, Screen.height / 2), "失败", null, null);
        }

        public void ShowLoading()
        {
            loading.SetActive(true);
        }

        public void DisableLoading()
        {
            loading.SetActive(false);
        }

        public void CreateSelectDlg(Vector3 position, string descript,string[] options,UI.SelectDialog.CallBack cbk)
        {
            GameObject dlgCanvas = Instantiate<GameObject>(selectDlg.gameObject.transform.parent.gameObject);
            dlgCanvas.SetActive(true);
            dlgCanvas.GetComponent<RectTransform>().parent = GetComponent<RectTransform>();
            GameObject dlg = dlgCanvas.GetComponentInChildren<UI.SelectDialog>().gameObject;
            dlg.GetComponent<RectTransform>().parent = dlgCanvas.GetComponent<RectTransform>();
            dlg.GetComponent<RectTransform>().localPosition = position;
            dlg.GetComponent<UI.SelectDialog>().Init(descript, options, cbk);
            
        }

        public void CreateItemDlg(string sname,string descript,string[] options,UI.ItemDialog.CallBack cbk)
        {
            GameObject dlgCanvas = Instantiate<GameObject>(itemDlg.gameObject.transform.parent.gameObject);
            dlgCanvas.SetActive(true);
            dlgCanvas.GetComponent<RectTransform>().parent = GetComponent<RectTransform>();
            Rect cvsrct = dlgCanvas.GetComponent<RectTransform>().rect;
            //Debug.Log(cvsrct);
            GameObject dlg = dlgCanvas.GetComponentInChildren<UI.ItemDialog>().gameObject;
            dlg.GetComponent<RectTransform>().parent = dlgCanvas.GetComponent<RectTransform>();
            dlg.GetComponent<RectTransform>().localPosition = new Vector3(Screen.width/2,Screen.height/2);
            dlg.GetComponent<UI.ItemDialog>().Init(sname, descript, options, cbk);

        }

        public UI.ConversationDialog CreateConversationDlgL(Vector3 position,string descript,string[] options,UI.ConversationDialog.CallBack cbk)
        {
            GameObject dlgCanvas = Instantiate<GameObject>(convDlgL.gameObject.transform.parent.gameObject);
            dlgCanvas.SetActive(true);
            dlgCanvas.GetComponent<RectTransform>().parent = GetComponent<RectTransform>();
            GameObject dlg = dlgCanvas.GetComponentInChildren<UI.ConversationDialog>().gameObject;
            dlg.GetComponent<RectTransform>().parent = dlgCanvas.GetComponent<RectTransform>();
            dlg.GetComponent<RectTransform>().localPosition = position;
            dlg.GetComponent<UI.ConversationDialog>().Init(descript, options, cbk);
            return dlg.GetComponent<UI.ConversationDialog>();
        }

        public UI.ConversationDialog CreateConversationDlgR(Vector3 position, string descript, string[] options, UI.ConversationDialog.CallBack cbk)
        {
            GameObject dlgCanvas = Instantiate<GameObject>(convDlgR.gameObject.transform.parent.gameObject);
            dlgCanvas.SetActive(true);
            dlgCanvas.GetComponent<RectTransform>().parent = GetComponent<RectTransform>();
            GameObject dlg = dlgCanvas.GetComponentInChildren<UI.ConversationDialog>().gameObject;
            dlg.GetComponent<RectTransform>().parent = dlgCanvas.GetComponent<RectTransform>();
            dlg.GetComponent<RectTransform>().localPosition = position;
            dlg.GetComponent<UI.ConversationDialog>().Init(descript, options, cbk);
            return dlg.GetComponent<UI.ConversationDialog>();
        }
    }
}