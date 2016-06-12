using UnityEngine;
using System.Collections;
namespace MemoryTrap
{
    namespace Test
    {
        public class MathTest : MonoBehaviour
        {
            public int start;
            public int end;
            public float lerp;
            // Use this for initialization
            public int srcX;
            public int srcY;
            public int dstX;
            public int dstY;
            void Start()
            {
                
            }

            public void LogLerp()
            {
                Debug.Log("lerp:" + Mathf.Lerp(start, end, lerp));
                Debug.Log("ToInt:" + (int)Mathf.Lerp(start, end, lerp));
            }

            public void LogVisible()
            {
                Debug.Log(MapManager.instance.maps[0].BarrierInLine(new Vector2I(srcX, srcY), new Vector2I(dstX, dstY),true));
            }

            // Update is called once per frame
            void Update()
            {

            }
        }
    }
}