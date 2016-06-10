using UnityEngine;
using System.Collections;
using TinyJSON;

namespace MemoryTrap
{
    public class MapBlock
    {
        //public Node node;
        protected GameObject _gameObject;
        [System.Serializable]
        public enum Dir
        {
            front,
            left,
            right,
            back
        }

        [System.Serializable]
        public enum Type
        {
            door,
            wall,
            wallCorner,
            floor,
            upStair,
            downStair,
            empty
        }

        protected Dir _direction = Dir.front;
        public Type type = Type.empty;
        public int idx = 0;
        public string style = "normal";
        public Dir direction
        {
            get { return _direction; }
            set {
                _direction = value;
                switch (_direction)
                {
                    case Dir.front:
                        if(_gameObject != null)
                            _gameObject.transform.localRotation = Quaternion.identity;
                        break;
                    case Dir.left:
                        if (_gameObject != null)
                            _gameObject.transform.localRotation = Quaternion.AngleAxis(-90, Vector3.up);
                        break;
                    case Dir.right:
                        if (_gameObject != null)
                            _gameObject.transform.localRotation = Quaternion.AngleAxis(90, Vector3.up);
                        break;
                    case Dir.back:
                        if (_gameObject != null)
                            _gameObject.transform.localRotation = Quaternion.AngleAxis(180, Vector3.up);
                        break;
                }
            }
        }
        
        public GameObject gameObject
        {
            get { return _gameObject; }
            set
            {
                _gameObject = value;
                switch (_direction)
                {
                    case Dir.front:
                        if (_gameObject != null)
                            _gameObject.transform.localRotation = Quaternion.identity;
                        break;
                    case Dir.left:
                        if (_gameObject != null)
                            _gameObject.transform.localRotation = Quaternion.AngleAxis(-90, Vector3.up);
                        break;
                    case Dir.right:
                        if (_gameObject != null)
                            _gameObject.transform.localRotation = Quaternion.AngleAxis(90, Vector3.up);
                        break;
                    case Dir.back:
                        if (_gameObject != null)
                            _gameObject.transform.localRotation = Quaternion.AngleAxis(180, Vector3.up);
                        break;
                }
            }
        }

        //需要在具体的block中实现
        public virtual void CreateObject(Vector2 pos,Transform parent)
        {
            return;
        }

        public virtual Node Serialize()
        {
            Node cur = Node.NewTable();
            cur["type"] = Node.NewInt((int)type);
            cur["dir"] = Node.NewInt((int)direction);
            cur["idx"] = Node.NewInt(idx);
            cur["style"] = Node.NewString(style);
            return cur;
        }

        public virtual void DeSerialize(Node node)
        {
            type = (Type)(int)node["type"];
            _direction = (Dir)(int)node["dir"];
            idx = (int)node["idx"];
            style = (string)node["style"];
        }

        ~MapBlock()
        {
            if (_gameObject != null)
            {
                MonoBehaviour.Destroy(_gameObject);
            }
        }
    }
}
