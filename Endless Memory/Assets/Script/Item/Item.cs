using UnityEngine;
using System.Collections;
using TinyJSON;
namespace MemoryTrap
{
    public abstract class Item
    {
        public abstract Node info { get; }
        public abstract Sprite sprite { get; }
        public abstract void TakeEffect();
        public abstract bool inSight { get; set; }
        public abstract GameObject prefab { get; }
        public abstract GameObject gameObject { get; }

    }
}