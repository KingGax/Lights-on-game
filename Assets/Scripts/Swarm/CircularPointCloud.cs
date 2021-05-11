#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CircularPointCloud : PointCloud {
    public float scannerCircleRadius;
    public Vector3 scannerBottom;
    public Vector3 scannerTop;
    public float yNumSteps;
    public float circleSamples;
    public bool showRaycastCentre;
    public bool showRaycasts;
    protected override Vector3[] CreatePointList() {
        Mesh colliderMesh = new Mesh();
        int pointsMissed = 0;
        MeshCollider tempCollider = gameObject.AddComponent<MeshCollider>();
        tempCollider.sharedMesh = mesh;
        List<Vector3> pointsList = new List<Vector3>();
        float angleInc = 2 * Mathf.PI / circleSamples;

        for (int i = 0; i < yNumSteps; i++) {
            float currentAngle = 0;
            for (int j = 0; j < circleSamples; j++) {
                RaycastHit hit;
                Vector3 centrePoint = Vector3.Lerp(scannerBottom, scannerTop, i / (yNumSteps-1));
                Vector3 rayStart = centrePoint + new Vector3(scannerCircleRadius * Mathf.Cos(currentAngle), 0, scannerCircleRadius * Mathf.Sin(currentAngle));
                Ray ray = new Ray(rayStart, centrePoint - rayStart);
                if (tempCollider.Raycast(ray, out hit, scannerCircleRadius)) {
                    pointsList.Add(hit.point + pointShift);
                }
                else {
                    pointsMissed++;
                }
                currentAngle += angleInc;
            }
        }
        Debug.Log("casting complete, total size: " + pointsList.Count + " points missed: " + pointsMissed);
        DestroyImmediate(tempCollider);

        return pointsList.ToArray();
    }

    protected override void OnDrawGizmos() {
        if (drawGizmos) {
            base.OnDrawGizmos();
            if (showRaycastCentre) {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(scannerBottom, 0.1f);
                Gizmos.DrawSphere(scannerTop, 0.1f);
                Gizmos.DrawLine(scannerBottom, scannerBottom + Vector3.left * scannerCircleRadius);
            }
            if (showRaycasts) {
                float angleInc = 2 * Mathf.PI / circleSamples;
                for (int i = 0; i < yNumSteps; i++) {
                    float currentAngle = 0;
                    for (int j = 0; j < circleSamples; j++) {
                        RaycastHit hit;
                        Vector3 centrePoint = Vector3.Lerp(scannerBottom, scannerTop, i / (yNumSteps-1));
                        Vector3 rayStart = centrePoint + new Vector3(scannerCircleRadius * Mathf.Cos(currentAngle), 0, scannerCircleRadius * Mathf.Sin(currentAngle));
                        Ray ray = new Ray(rayStart, centrePoint - rayStart);
                        Gizmos.DrawRay(ray);
                        currentAngle += angleInc;
                    }
                }
            }
        }
    }

}
#else
public class CircularPointCloud : PointCloud {}
#endif