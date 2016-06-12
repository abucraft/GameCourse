using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace MemoryTap
{
    namespace UI
    {
        [ExecuteInEditMode]
        [RequireComponent(typeof(RectTransform))]
        public class UIAsCameraFoward : MonoBehaviour
        {

            // Use this for initialization
            void Start()
            {

            }

            // Update is called once per frame
            void LateUpdate()
            {
                Quaternion rotate = Camera.main.transform.rotation;
                RectTransform rctTrans = GetComponent<RectTransform>();
                rctTrans.rotation = rotate;
            }
        }
    }
}