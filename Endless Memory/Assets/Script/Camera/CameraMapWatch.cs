using UnityEngine;
using System.Collections;
namespace MemoryTrap
{
    public class CameraMapWatch : MonoBehaviour
    {

        public MainCharactor focusCharactor;
        /*public Texture2D fowTex;
        public GameObject fow;
        public int sample = 4;*/
        // Use this for initialization
        void Start()
        {
        }

        public void RefreshCameraRect()
        {
            MapManager.instance.ShowCameraRect(GetCameraRect(), focusCharactor.curLevel);
        }

        //获取到视野内的方块区域
        public Rect GetCameraRect()
        {
            /*
             * ScreenArea:
             * leftBtmR ----------- rightBtmR
             * |                            |
             * |                            |
             * |                            |
             * leftTopR ----------- rightTopR
             */

            Ray leftTopR = Camera.main.ScreenPointToRay(new Vector2(0, 0));
            Ray leftBtmR = Camera.main.ScreenPointToRay(new Vector2(0, Screen.height));
            Ray rightBtmR = Camera.main.ScreenPointToRay(new Vector2(Screen.width, Screen.height));
            float fieldOfView = Camera.main.fieldOfView;
            float height = Camera.main.transform.position.y - focusCharactor.curLevel;
            float rotateX = Camera.main.transform.rotation.eulerAngles.x;
            float angle = rotateX - fieldOfView / 2;
            float angle2 = rotateX + fieldOfView / 2;
            float length = Mathf.Abs(height / Mathf.Sin(angle * Mathf.Deg2Rad));
            float length2 = Mathf.Abs(height / Mathf.Sin(angle2 * Mathf.Deg2Rad));
            Vector3 leftTop = leftTopR.GetPoint(length2);
            Vector3 leftBtm = leftBtmR.GetPoint(length);
            Vector3 rightBtm = rightBtmR.GetPoint(length);
            float tmpWidth = rightBtm.x - leftBtm.x;
            float tmpHeight = height/Mathf.Tan(angle * Mathf.Deg2Rad);
            return new Rect(leftBtm.x - 0.25f*tmpWidth, leftTop.z - 0.25f*tmpHeight, tmpWidth*1.5f, tmpHeight*1.5f);
        }

        // Update is called once per frame
        void Update()
        {
            
            //adjust showing map per frame
            //Debug.Log(GetCameraRect());
            MapManager.instance.ShowCameraRect(GetCameraRect(),focusCharactor.curLevel);
        }
    }
}