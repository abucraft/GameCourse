using UnityEngine;
using System.Collections;

namespace MemoryTrap
{
    public class MapBlock : MonoBehaviour
    {
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
            step,
            empty
        }

        Dir _direction = Dir.front;
        public Type type = Type.empty;
        public Dir direction
        {
            get { return _direction; }
            set {
                _direction = value;
                switch (_direction)
                {
                    case Dir.front:
                        transform.rotation = Quaternion.identity;
                        break;
                    case Dir.left:
                        transform.rotation = Quaternion.AngleAxis(-90, Vector3.up);
                        break;
                    case Dir.right:
                        transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
                        break;
                    case Dir.back:
                        transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
                        break;
                }
            }
        }
    }
}
