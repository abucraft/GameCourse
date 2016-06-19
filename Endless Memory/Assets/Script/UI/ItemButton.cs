using UnityEngine;
using UnityEngine.UI;
using TinyJSON;
using System.Collections;
namespace MemoryTrap
{
    namespace UI
    {
        [RequireComponent(typeof(Image))]
        [RequireComponent(typeof(Button))]
        public class ItemButton : MonoBehaviour
        {
            public static Sprite originSprite;
            public int idx;
            // Use this for initialization
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            void ResetUI()
            {
                ItemPack itmPack = GameManager.instance.mainCharactor.items[idx];
                if(itmPack == null)
                {
                    GetComponent<Image>().sprite = originSprite;
                    GetComponentInChildren<Text>().text = "0";
                }
                else
                {
                    GetComponent<Image>().sprite = itmPack.item.sprite;
                    GetComponentInChildren<Text>().text = itmPack.count.ToString();
                }
            }

            void OnEnable()
            {
                if (originSprite == null)
                {
                    originSprite = GetComponent<Image>().sprite;
                }
                ItemPack itmPack = GameManager.instance.mainCharactor.items[idx];
                if(itmPack != null)
                {
                    Item itm = itmPack.item;
                    Sprite tex = itm.sprite;
                    GetComponent<Image>().sprite = tex;
                    Text countText = GetComponentInChildren<Text>();
                    if(countText != null)
                    {
                        countText.text = itmPack.count.ToString();
                    }
                    Node info = itm.info;
                    GetComponent<Button>().onClick.RemoveAllListeners();
                    GetComponent<Button>().onClick.AddListener(() =>
                    {
                        string[] options = new string[2];
                        options[0] = StringResource.use;
                        options[1] = StringResource.cancel;
                        UIManager.instance.CreateItemDlg((string)info["name"], (string)info["description"], options, (int i) =>
                        {
                            if(i == 0)
                            {
                                GameManager.instance.mainCharactor.UseItem(idx);
                                ResetUI();
                            }
                        });
                    });
                }
            }
        }
    }
}