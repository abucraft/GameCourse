using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace MemoryTrap
{
    public class MapBlockFactory : MonoBehaviour
    {
        public MapBlock.Type type;
        [HideInInspector]
        public StringObjectsDictionary objCollection = new StringObjectsDictionary(); 
        public GameObject getObject(Vector2 pos,MapBlock.Dir dir,string style)
        {
            System.Type blockType = null;
            switch (type)
            {
                case MapBlock.Type.door:
                    blockType = typeof(Door);
                    break;
                case MapBlock.Type.floor:
                    blockType = typeof(Floor);
                    break;
                case MapBlock.Type.empty:
                    blockType = typeof(Empty);
                    break;
                case MapBlock.Type.step:
                    blockType = typeof(Step);
                    break;
                case MapBlock.Type.wall:
                    blockType = typeof(Wall);
                    break;
                case MapBlock.Type.wallCorner:
                    blockType = typeof(WallCorner);
                    break;
            }
            //Debug.Log(type.ToString());
            GameObject[] objs = objCollection[style].objects;
            Debug.Assert(objs.Length != 0,"factory length 0");
            int select = Random.Range(0, objs.Length);
            GameObject newObj = (GameObject)Instantiate(objs[select],new Vector3(pos.x,0,-pos.y),Quaternion.identity);
            newObj.AddComponent(blockType);
            MapBlock block = (MapBlock)newObj.GetComponent(blockType);
            block.direction = dir;
            return newObj;
        }
    }
}