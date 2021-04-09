using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveableVector3 {
    public float x;
    public float y;
    public float z;
}

[Serializable]
public class PointCloudData 
{
    public PointCloudData(Vector3[] ps) {
        points = new SaveableVector3[ps.Length];
        for (int i = 0; i < points.Length; i++) {
            points[i] = new SaveableVector3 {
                x = ps[i].x,
                y = ps[i].y,
                z = ps[i].z
            };
        }
    }

    public Vector3[] GetPoints() {
        Vector3[] vectorPoints = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++) {
            vectorPoints[i].x = points[i].x;
            vectorPoints[i].y = points[i].y;
            vectorPoints[i].z = points[i].z;
        }
        return vectorPoints;
    }
    [SerializeField]
    public SaveableVector3[] points;
}
