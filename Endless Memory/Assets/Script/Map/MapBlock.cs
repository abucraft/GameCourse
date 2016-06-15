using UnityEngine;
using System.Collections;
using TinyJSON;

namespace MemoryTrap
{
    public class MapBlock
    {
        //public Node node;
        protected GameObject _gameObject;
        //ui to display info
        protected GameObject _uiCanvas;
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
        protected bool _visited = false;
        protected bool _inSight = false;
        protected bool _available = false;
        protected bool _selected = false;
        public virtual bool visited
        {
            get
            {
                return _visited;
            }
            set
            {
                if(_visited != value)
                {
                    _visited = value;
                    ChangeVisibility();
                }
                
            }
        }

        public virtual bool inSight
        {
            get
            {
                return _inSight;
            }
            set
            {
                if (_inSight != value)
                {
                    _inSight = value;
                    ChangeVisibility();
                }
            }
        }

        public virtual bool available {
            get
            {
                return _available;
            }
            set
            {
                if (_available != value)
                {
                    _available = value;
                    ChangeDisplay();
                }
            }
        }

        public virtual bool selected
        {
            get
            {
                return _selected;
            }
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    ChangeDisplay();
                }
            }
        }

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
                ChangeVisibility();
                ChangeDisplay();
            }
        }

        public virtual void ChangeDisplay()
        {
            //Debug.Log("change available color");
            if (_gameObject != null)
            {
                MeshRenderer[] renders = _gameObject.GetComponentsInChildren<MeshRenderer>();
                if (_selected)
                {
                    Color sColor = MapManager.instance.selectColor;
                    for(int i = 0; i < renders.Length; i++)
                    {
                        renders[i].material.color = sColor;
                    }
                    return;
                }
                if (_available)
                {
                    
                    Color aColor = MapManager.instance.availableColor;
                    for (int i = 0; i < renders.Length; i++)
                    {
                        renders[i].material.color = aColor;
                    }
                    return;
                }
                for(int i = 0; i < renders.Length; i++)
                {
                    renders[i].material.color = Color.white;
                }
                return;
            }
        }

        public virtual void ChangeVisibility()
        {
            if (_gameObject != null)
            {
                MeshRenderer[] renders = _gameObject.GetComponentsInChildren<MeshRenderer>();
                if (_inSight)
                {
                    Shader sInSight = Shader.Find("MapBlock/InSight");
                    for(int i = 0; i < renders.Length; i++)
                    {
                        renders[i].material.shader = sInSight;
                    }
                    return;
                }
                if (_visited)
                {
                    Shader sVisited = Shader.Find("MapBlock/Visited");
                    for(int i = 0; i < renders.Length; i++)
                    {
                        renders[i].material.shader = sVisited;
                    }
                    return;
                }
                Shader sHide = Shader.Find("MapBlock/Hide");
                for(int i = 0; i < renders.Length; i++)
                {
                    renders[i].material.shader = sHide;
                }
                return;
            }
        }

        //显示Block信息
        public virtual void DisplayInfo(string addition)
        {
            if (_gameObject == null)
            {
                return;
            }
            if (_uiCanvas == null)
            {
                _uiCanvas = GameObject.Instantiate<GameObject>(UIManager.instance.objectCanvas);
                _uiCanvas.transform.parent = _gameObject.transform;
                _uiCanvas.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, -1);
                string info = addition + "type: " + type + '\n' + "direction: " + direction + '\n' + "style: " + style;
                _uiCanvas.GetComponent<ObjectUI>().dialog.text = info;
            }
        }

        //不显示信息
        public virtual void HideInfo()
        {
            if(_gameObject== null)
            {
                return;
            }
            if (_uiCanvas != null)
            {
                GameObject.Destroy(_uiCanvas);
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
            cur["visited"] = Node.NewBool(visited);
            return cur;
        }

        public virtual void DeSerialize(Node node)
        {
            type = (Type)(int)node["type"];
            _direction = (Dir)(int)node["dir"];
            idx = (int)node["idx"];
            style = (string)node["style"];
            visited = (bool)node["visited"];
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
