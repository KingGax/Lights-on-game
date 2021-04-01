using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (PointCloud))]
public class MeshPointBaker : Editor
{
    public void OnEnable() {
        PointCloud script = (PointCloud)target;
        script.Start();
    }
    public override void OnInspectorGUI() {
        PointCloud script = (PointCloud)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Load File")) {
            script.Start();
        }
        if (GUILayout.Button("Generate")) {
            script.GeneratePoints();
        }
    }
}
