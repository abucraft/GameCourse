using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace MemoryTrap
{
    public class UIManager : MonoBehaviour
    {
        public GameObject loading;
        public GameObject objectCanvas;
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
        }

        public void ShowLoading()
        {
            loading.SetActive(true);
        }

        public void DisableLoading()
        {
            loading.SetActive(false);
        }
    }
}