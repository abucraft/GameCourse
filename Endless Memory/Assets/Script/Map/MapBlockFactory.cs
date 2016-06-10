using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TinyJSON;
namespace MemoryTrap
{
    public class MapBlockFactory : MonoBehaviour
    {
        public MapBlock.Type type;
        [HideInInspector]
        public StringObjectsDictionary objCollection = new StringObjectsDictionary();
        
        public GameObject getObject(string style)
        {
            
            //Debug.Log(type.ToString());
            GameObject[] objs = objCollection[style].objects;
            Debug.Assert(objs.Length != 0,"factory length 0");
            int select = Random.Range(0, objs.Length);
            GameObject newObj = (GameObject)Instantiate(objs[select],new Vector3(0,0),Quaternion.identity);
            return newObj;
        }
    }
}