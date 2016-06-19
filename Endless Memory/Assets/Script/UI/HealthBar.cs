using UnityEngine;
using UnityEngine.UI;
using System.Collections;
namespace MemoryTrap
{
    namespace UI
    {
        public class HealthBar : MonoBehaviour
        {
            public Text text;
            public RectTransform frameBar;
            public RectTransform valueBar;
            [SerializeField]
            protected int _scale;
            protected int _totalValue = 0;
            protected int _restValue = 0;
            
            public int scale
            {
                get
                {
                    return _scale;
                }
                set
                {
                    _scale = value;
                    Vector2 restSize = valueBar.sizeDelta;
                    valueBar.sizeDelta = new Vector2(_restValue * _scale, restSize.y);
                    Vector2 frameSize = frameBar.sizeDelta;
                    frameBar.sizeDelta = new Vector2(_totalValue * _scale, frameSize.y);
                }
            }
            public int totalValue
            {
                get
                {
                    return _totalValue;
                }
                set
                {
                    _totalValue = value;
                    Vector2 frameSize = frameBar.sizeDelta;
                    frameBar.sizeDelta = new Vector2(_totalValue * _scale, frameSize.y);
                    string values = _restValue.ToString() + '/' + _totalValue.ToString();
                    text.text = values;
                }
            }

            public int restValue
            {
                get
                {
                    return _restValue;
                }
                set
                {
                    _restValue = value;
                    Vector2 restSize = valueBar.sizeDelta;
                    valueBar.sizeDelta = new Vector2(_restValue * _scale, restSize.y);
                    string values = _restValue.ToString() + '/' + _totalValue.ToString();
                    text.text = values;
                }
            }

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
}