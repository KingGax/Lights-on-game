using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEditor;

public class PointCloud : MonoBehaviour {
    public bool showPoints;
    public bool showRaycastCentre;
    public Mesh mesh;
    [SerializeField]
    private Vector3[] points;
    public Vector3 pointShift;
    public PointCloudSO cloudPrefab;
    public float scannerCircleRadius;
    public Vector3 scannerBottom;
    public Vector3 scannerTop;
    public float yNumSteps;
    public float circleSamples;
    public bool drawGizmos = false;
    // Start is called before the first frame update
    public void LoadFile() {
        if (cloudPrefab != null) {
            points = cloudPrefab.points;
        }
        else {
            Debug.LogError("Set the cloudPrefab to be a scriptable object");
        }
    }

    public void GeneratePoints() {

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
                Vector3 centrePoint = Vector3.Lerp(scannerBottom, scannerTop, i / yNumSteps);
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
        points = pointsList.ToArray();
        DestroyImmediate(tempCollider);
        SaveFile();
    }

    private void SaveFile() {
        if (cloudPrefab != null) {
            cloudPrefab.points = points;
            EditorUtility.SetDirty(cloudPrefab);
        }
        else {
            Debug.LogError("No cloud prefab set, changes will not be saved");
        }
        
    }


    private void OnDrawGizmos() {
        if (drawGizmos) {
            if (showPoints) {
                Gizmos.color = Color.white;
                if (points != null) {
                    foreach (Vector3 point in points) {
                        Gizmos.DrawSphere(point, 0.1f);
                    }
                }
            }
            if (showRaycastCentre) {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(scannerBottom, 0.1f);
                Gizmos.DrawSphere(scannerTop, 0.1f);
                Gizmos.DrawLine(scannerBottom, scannerBottom + Vector3.left * scannerCircleRadius);
            }
        }
    }

}
