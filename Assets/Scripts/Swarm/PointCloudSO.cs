using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PointCloudSO", order = 1)]
public class PointCloudSO : ScriptableObject
{
    public Vector3[] points;
    public Quaternion initialRotation;
}
