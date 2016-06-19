using UnityEngine;
using System.Collections;
using System;
using TinyJSON;

namespace MemoryTrap
{
    public class Apple : Item
    {
        protected static Sprite _img;
        protected static GameObject _prefab;
        protected static GameObject _object;
        protected bool _inSight;
        public override GameObject gameObject
        {
            get
            {
                return _object;
            }
        }

        public override Node info
        {
            get
            {
                Node _info = Node.NewTable();
                _info["name"] = Node.NewString(StringResource.AppleName);
                _info["description"] = Node.NewString(StringResource.AppleDescription);
                return _info;
            }
        }

        public override bool inSight
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
                    if (_inSight)
                    {
                        if (_object == null)
                        {
                            _object = MonoBehaviour.Instantiate<GameObject>(prefab);
                            ItemHolder hd = _object.GetComponent<ItemHolder>();
                            if (hd == null)
                            {
                                Debug.LogError("HPBottle: no gameobject");
                            }
                            else
                            {
                                hd.item = this;
                            }
                        }
                    }
                    else
                    {
                        if (_object != null)
                        {
                            MonoBehaviour.Destroy(_object);
                        }
                    }
                }
            }
        }

        public override Sprite sprite
        {
            get
            {
                if (_img == null)
                {
                    _img = Resources.Load<Sprite>("RPG_inventory_icons/apple");
                }
                return _img;
            }
        }

        public override GameObject prefab
        {
            get
            {
                if (_prefab == null)
                {
                    _prefab = Resources.Load<GameObject>("RPG_item_prefabs/Apple");
                }
                return _prefab;
            }
        }

        public override void TakeEffect()
        {
            MainCharactor main = GameManager.instance.mainCharactor;
            main.step += 1;
        }
        ~Apple()
        {
            if (_object != null)
            {
                MonoBehaviour.Destroy(_object);
            }
        }
    }
}