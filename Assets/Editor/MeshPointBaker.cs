using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (PointCloud),true)]
public class MeshPointBaker : Editor
{
    public void OnEnable() {
        PointCloud script = (PointCloud)target;
        script.LoadFile();
        script.ShowGizmos();
    }

    public void OnDisable() {
        PointCloud script = (PointCloud)target;
        script.HideGizmos();
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
