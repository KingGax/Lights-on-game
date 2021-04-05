using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEditor;

public abstract class PointCloud : MonoBehaviour {
    public bool showPoints;
    public Mesh mesh;
    [SerializeField]
    protected Vector3[] points;
    public Vector3 pointShift;
    public PointCloudSO cloudPrefab;

    protected bool drawGizmos = false;
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
        points = CreatePointList();
        SaveFile();
    }
    public void HideGizmos() {
        drawGizmos = false;
    }

    public void ShowGizmos() {
        drawGizmos = true;
    }
    protected void DrawPoints() {
        if (showPoints) {
            Gizmos.color = Color.white;
            if (points != null) {
                foreach (Vector3 point in points) {
                    Gizmos.DrawSphere(point + transform.position, 0.1f);
                }
            }
        }
    }
    protected virtual void OnDrawGizmos() {
        if (drawGizmos) {
            DrawPoints();
        }
    }

    protected abstract Vector3[] CreatePointList();

    private void SaveFile() {
        if (cloudPrefab != null) {
            cloudPrefab.points = points;
            EditorUtility.SetDirty(cloudPrefab);
        }
        else {
            Debug.LogError("No cloud prefab set, changes will not be saved");
        }
        
    }

    

}
