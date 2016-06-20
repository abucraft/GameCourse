using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MemoryTrap
{
    namespace UI
    {
        [RequireComponent(typeof(Text))]
        public class CollectionHint : MonoBehaviour
        {
            public float lifeTime = 1f;
            public Text hintTex;
            private float curTime = 0f;
            // Use this for initialization
            public void Init(string hint)
            {
                hintTex.text = hint;
            }

            // Update is called once per frame
            void Update()
            {
                if (curTime >= lifeTime)
                {
                    Destroy(gameObject);
                }
                curTime += Time.deltaTime;
            }
        }
    }
}