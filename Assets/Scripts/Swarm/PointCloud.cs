#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public abstract class PointCloud : MonoBehaviour {
    public bool showPoints;
    public Mesh mesh;
    [SerializeField]
    protected Vector3[] points;
    public Vector3 pointShift;
    public PointCloudSO cloudPrefab;
    protected Quaternion defaultRotation;
    public Transform rotationParent;

    protected bool drawGizmos = false;

    public void LoadFile() {
        if (cloudPrefab != null) {
            points = cloudPrefab.points;
            defaultRotation = cloudPrefab.initialRotation;
        } else {
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
            Quaternion defaultQuat = cloudPrefab.initialRotation;
            Transform rotTransform;
            if (rotationParent != null) {
                rotTransform = rotationParent;
            } else {
                rotTransform = transform;
            }
            Quaternion rotationQuat = rotTransform.localRotation * Quaternion.Inverse(defaultQuat); 
            if (points != null) {
                foreach (Vector3 point in points) {
                    Gizmos.DrawSphere(transform.position + rotationQuat * (point), 0.1f);
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
            if (rotationParent != null) {
                cloudPrefab.initialRotation = rotationParent.localRotation;
            } else {
                rotationParent = transform;
                cloudPrefab.initialRotation = transform.localRotation;
            }
            
            EditorUtility.SetDirty(cloudPrefab);
        } else {
            Debug.LogError("No cloud prefab set, changes will not be saved");
        }
    }
}
#else
using UnityEngine;
public abstract class PointCloud : MonoBehaviour {}
#endif