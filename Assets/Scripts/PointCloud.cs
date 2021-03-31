using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloud : MonoBehaviour {
    public bool showSphere;
    public bool showPoints;
    public bool onlyTopHalf;
    public int numPoints;
    //public SkinnedMeshRenderer smr;
    public Mesh mesh;
    [SerializeField]
    private Vector3[] points;
    public float sphereRadius;
    public Vector3 sphereOffset;
    public Vector3 pointShift;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    Vector3[] PointsOnSphere(int n) {
        List<Vector3> upts = new List<Vector3>();
        float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
        float off = 2.0f / n;
        float x = 0;
        float y = 0;
        float z = 0;
        float r = 0;
        float phi = 0;

        for (var k = 0; k < n; k++) {
            y = k * off - 1 + (off / 2);
            r = Mathf.Sqrt(1 - y * y);
            phi = k * inc;
            x = Mathf.Cos(phi) * r;
            z = Mathf.Sin(phi) * r;
            if (onlyTopHalf) {
                if (y < 0) {
                    y *= -1;
                }
            }
            upts.Add(new Vector3(x, y, z));
        }
        Vector3[] pts = upts.ToArray();
        return pts;
    }


    public void GeneratePoints() {
        points = PointsOnSphere(numPoints);

        Mesh colliderMesh = new Mesh();
        
        MeshCollider tempCollider = gameObject.AddComponent<MeshCollider>();
        tempCollider.sharedMesh = mesh;

        for (int i = 0; i < points.Length; i++) {
            RaycastHit hit;

            points[i] = points[i] * sphereRadius + sphereOffset;
            Ray ray = new Ray(points[i], transform.position - points[i]);
            if (tempCollider.Raycast(ray, out hit, 10f)) {
                points[i] = hit.point + pointShift;
            }
            else {
                Debug.LogError("didnt hit anything :(");
            }
        }
        DestroyImmediate(tempCollider);
    }

    private void OnDrawGizmos() {
        if (showSphere) {
            Gizmos.DrawSphere(sphereOffset, sphereRadius);
        }
        if (showPoints) {
            if (points != null) {
                foreach (Vector3 point in points) {
                    Gizmos.DrawSphere(point, 0.1f);
                }
            }
        }
    }

}
