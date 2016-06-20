using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace MemoryTrap
{
    namespace UI
    {
        public class MakeBlockPanel : MonoBehaviour
        {
            public GameObject pannel;
            // Use this for initialization
            void Start()
            {
                RectTransform rcttrans = pannel.GetComponent<RectTransform>();
                rcttrans.localPosition = new Vector3(Screen.width / 2, Screen.height / 2);
            }

            // Update is called once per frame
            void Update()
            {

            }
        }
    }
}