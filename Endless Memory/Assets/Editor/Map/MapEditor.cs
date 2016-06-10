using UnityEngine;
using UnityEditor;
using System.Collections;
namespace MemoryTrap
{
    [CustomEditor(typeof(Map))]
    [ExecuteInEditMode]
    public class MapEditor : Editor
    {
        int ptLength = 0;
        bool ptFold = true;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            {
                Map map = (Map)target;
                ptLength = map.roomPatterns.Length;
                ptFold = EditorGUILayout.Foldout(ptFold, "Room Patterns");
                if (ptFold)
                {

                    EditorGUI.indentLevel++;
                    ptLength = EditorGUILayout.IntField("Size", ptLength);
                    //长度不相等时拷贝一份
                    if (ptLength != map.roomPatterns.Length)
                    {
                        Texture2D[] ptArray = new Texture2D[ptLength];

                        for (int i = 0; i < ptArray.Length; i++)
                        {
                            if (i < map.roomPatterns.Length)
                            {
                                ptArray[i] = map.roomPatterns[i];
                            }
                        }
                        map.roomPatterns = ptArray;
                    }
                    //显示map中roomPattern的图片
                    for (int i = 0; i < map.roomPatterns.Length; i++)
                    {
                        map.roomPatterns[i] = EditorGUILayout.ObjectField("Pattern " + i.ToString(), map.roomPatterns[i], typeof(Texture2D), true) as Texture2D;
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }
    }
}
