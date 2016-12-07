using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Map))]
public class MapModelCreator : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if(GUILayout.Button("Create Map Model"))
        {
            var map = target as Map;
            map.CreateModel();
        }
    }
}
