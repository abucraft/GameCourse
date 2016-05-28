using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
namespace MemoryTrap
{
    [CustomEditor(typeof(MapBlockFactory))]
    public class MapBlockFactoryEditor : Editor
    {
        public bool foldObjects = true;
        SerializedProperty objCollection;
        void OnEnable()
        {
            objCollection = serializedObject.FindProperty("objCollection");
        }
         
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            foldObjects = EditorGUILayout.Foldout(foldObjects, "objects");
            if (foldObjects)
            {
                EditorGUI.indentLevel++;
                StringObjectsDictionary dict = ((MapBlockFactory)target).objCollection;
                
                StringObjectsDictionary.KeyCollection keys = dict.Keys;
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
                    GameObject[] objs = dict[name].objects;
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
                        dict[name] = new ObjectsList(newObjs);
                    }
                    //换个新名字
                    if (nname != name)
                    {
                        objs = dict[name].objects;
                        GameObject[] newObjs = new GameObject[objs.Length];
                        for(int i = 0; i < objs.Length; i++)
                        {
                            newObjs[i] = objs[i];
                        }
                        dict.Add(nname, new ObjectsList(newObjs));
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
                        dict.Add("unnamed", new ObjectsList(array));
                    }
                }
                serializedObject.CopyFromSerializedProperty(new SerializedObject(target).FindProperty("objCollection"));
                EditorGUI.indentLevel--;

            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}