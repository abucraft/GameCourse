using UnityEngine;
using UnityEditor;
using NUnit.Framework;
namespace MemoryTrap
{
    [CustomEditor(typeof(DictionaryTest))]
    public class DictionaryTestEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

        }

    }
}