using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
namespace MemoryTrap
{
    [CustomEditor(typeof(MapBlockFactory))]
    public class MapBlockFactoryEditor : Editor
    {
        public string[] options = new string[] { "Door", "Floor", "Wall","WallCorner","Step" };
        public int index = 0;
        public bool foldObjects = true;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            string select = options[index];
            MapBlockFactory factory = (MapBlockFactory)target;
            
            if(factory.type == typeof(Door))
            {
                index = 0;
            }
            if (factory.type == typeof(Floor))
            {
                index = 1;
            }
            if( factory.type == typeof(Wall))
            {
                index = 2;
            }
            if( factory.type == typeof(WallCorner))
            {
                index = 3;
            }
            if(factory.type == typeof(Step))
            {
                index = 4;
            }
            index = EditorGUILayout.Popup(index, options);
            switch (select)
            {
                case "Door":
                    factory.type = typeof(Door);
                    break;
                case "Floor":
                    factory.type = typeof(Floor);
                    break;
                case "Wall":
                    factory.type = typeof(Wall);
                    break;
                case "WallCorner":
                    factory.type = typeof(WallCorner);
                    break;
                case "Step":
                    factory.type = typeof(Step);
                    break;
            }
            foldObjects = EditorGUILayout.Foldout(foldObjects, "objects");
            if (foldObjects)
            {
                EditorGUI.indentLevel++;
                SerializableDictionary<string, GameObject[]> dict = factory.objCollection;
                SerializableDictionary<string, GameObject[]>.KeyCollection keys = dict.Keys;
                List<string> removeList = new List<string>();
                List<string> nameList = new List<string>();
                //由于在dict的iterator 中不能改变dict内容，因此先取出所有key
                foreach (string name in keys)
                {
                    nameList.Add(name);
                }
                foreach(string name in nameList) { 
                    string nname = EditorGUILayout.TextField("style name",name);
                    EditorGUI.indentLevel++;
                    GameObject[] objs = dict[name];
                    int ptLength = objs.Length;
                    ptLength = EditorGUILayout.IntField("Size", ptLength);
                    //长度不同拷贝一份
                    if(ptLength!= objs.Length)
                    {
                        GameObject[] newObjs = new GameObject[ptLength];
                        for (int i = 0; i < ptLength; i++)
                        {
                            if (i < objs.Length)
                            {
                                newObjs[i] = objs[i];
                            }
                        }
                        dict[name] = newObjs;
                    }
                    //换个新名字
                    if (nname != name)
                    {
                        objs = dict[name];
                        GameObject[] newObjs = new GameObject[objs.Length];
                        for(int i = 0; i < objs.Length; i++)
                        {
                            newObjs[i] = objs[i];
                        }
                        dict.Add(nname, newObjs);
                        removeList.Add(name);
                    }
                    for(int i = 0; i < objs.Length; i++)
                    {
                        objs[i] = (GameObject)EditorGUILayout.ObjectField(objs[i], typeof(GameObject), true);
                    }
                    //删除按钮，删除该项
                    if (GUILayout.Button("删除"))
                    {
                        removeList.Add(name);
                    }
                    EditorGUI.indentLevel--;
                    
                }
                for(int i = 0;i< removeList.Count; i++)
                {
                    if (dict.ContainsKey(removeList[i]))
                    {
                        dict.Remove(removeList[i]);
                    }
                }
                if (GUILayout.Button("添加")){
                    GameObject[] array = new GameObject[0];
                    if (!dict.ContainsKey("unnamed"))
                    {
                        dict.Add("unnamed", array);
                    }
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}