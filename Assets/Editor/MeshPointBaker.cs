using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (PointCloud))]
public class MeshPointBaker : Editor
{
    public void OnEnable() {
        PointCloud script = (PointCloud)target;
        script.LoadFile();
        script.drawGizmos = true;
    }

    public void OnDisable() {
        PointCloud script = (PointCloud)target;
        script.drawGizmos = false;
    }
    public override void OnInspectorGUI() {
        PointCloud script = (PointCloud)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Load File")) {
            script.LoadFile();
        }
        if (GUILayout.Button("Generate")) {
            script.GeneratePoints();
        }
    }
}
