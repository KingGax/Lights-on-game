using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (PointCloud))]
public class MeshPointBaker : Editor
{
    public override void OnInspectorGUI() {
        //base.OnInspectorGUI();
        PointCloud script = (PointCloud)target;
        DrawDefaultInspector();
         
        if (GUILayout.Button("Generate")) {
            script.GeneratePoints();
        }
    }
}
