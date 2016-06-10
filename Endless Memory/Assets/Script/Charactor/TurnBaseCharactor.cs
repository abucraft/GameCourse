using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MemoryTrap
{
    //需要自行获取地图，可以修改AppManager和GameManager
    public abstract class TurnBaseCharactor : MonoBehaviour
    {
        //get where the charactor can go
        public abstract List<Vector2> getReachableArea();

        //get the path to a position
        public abstract List<Vector2> getPathTo(Vector2 endPos);
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
