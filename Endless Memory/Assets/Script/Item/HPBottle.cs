using UnityEngine;
using System.Collections;
using System;
using TinyJSON;

namespace MemoryTrap
{
    public class HPBottle : Item
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
                _info["name"] = Node.NewString(StringResource.HPBottleName);
                _info["description"] = Node.NewString(StringResource.HPBottleDescription);
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
                if(_inSight!= value)
                {
                    _inSight = value;
                    if (_inSight)
                    {
                        if(_object == null)
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
                if(_img == null)
                {
                    _img = Resources.Load<Sprite>("RPG_inventory_icons/hp");
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
                    _prefab = Resources.Load<GameObject>("RPG_item_prefabs/HPBottle");
                }
                return _prefab;
            }
        }

        public override void TakeEffect()
        {
            MainCharactor main = GameManager.instance.mainCharactor;
            int curHp = main.curHp;
            int maxHp = main.hp;
            curHp += 50;
            curHp = curHp > maxHp ? maxHp : curHp;
            main.curHp = curHp;
        }
        ~HPBottle()
        {
            if (_object != null)
            {
                MonoBehaviour.Destroy(_object);
            }
        }
    }
}