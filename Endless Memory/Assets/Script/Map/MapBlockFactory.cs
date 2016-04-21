using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace MemoryTrap
{
    public class MapBlockFactory : MonoBehaviour
    {
        
        public System.Type type = null;
        public SerializableDictionary<string, GameObject[]> objCollection = new SerializableDictionary<string, GameObject[]>(); 
        public GameObject getObject(Vector2 pos,MapBlock.Dir dir,string style)
        {
            Debug.Assert(type != null);
            //Debug.Log(type.ToString());
            GameObject[] objs = objCollection[style];
            Debug.Assert(objs.Length != 0,"factory length 0");
            int select = Random.Range(0, objs.Length);
            GameObject newObj = (GameObject)Instantiate(objs[select],new Vector3(pos.x,pos.y),Quaternion.identity);
            newObj.AddComponent(type);
            MapBlock block = (MapBlock)newObj.GetComponent(type);
            block.direction = dir;
            return newObj;
        }
    }
}