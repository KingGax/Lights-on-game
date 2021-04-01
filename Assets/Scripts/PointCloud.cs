using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class PointCloud : MonoBehaviour {
    public bool showSphere;
    public bool showPoints;
    public bool onlyTopHalf;
    public bool showRaycastCentre;
    public int numPoints;
    //public SkinnedMeshRenderer smr;
    public Mesh mesh;
    [SerializeField]
    private Vector3[] points;
    public float sphereRadius;
    public Vector3 sphereOffset;
    public Vector3 pointShift;
    public Vector3[] raycastCentres;
    public PointCloudSO cloudPrefab;
    // Start is called before the first frame update
    public void Start() {
        if (cloudPrefab != null) {
            points = cloudPrefab.points;
        }
        else {
            Debug.LogError("Set the cloudPrefab to be a scriptable object");
        }
        if (raycastCentres == null) {
            raycastCentres = new Vector3[] { transform.position };
        }
        /*try {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path + fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            PointCloudData obj = (PointCloudData)formatter.Deserialize(stream);
            stream.Close();
            points = obj.GetPoints();
            Debug.Log("File " + fileName + " loaded successfully");
        }
        catch (System.Exception e) {
            Debug.LogError(e.Message);
        }*/
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

        if (raycastCentres.Length == 0) {
            raycastCentres = new Vector3[] { transform.position };
        }

        Mesh colliderMesh = new Mesh();
        int pointsMissed = 0;
        MeshCollider tempCollider = gameObject.AddComponent<MeshCollider>();
        tempCollider.sharedMesh = mesh;
        List<Vector3> pointsList = new List<Vector3>();
        for (int i = 0; i < points.Length; i++) {
            RaycastHit hit;

            points[i] = points[i] * sphereRadius + sphereOffset;
            Vector3 raycastCentre = GetClosestCentre(points[i]);
            Ray ray = new Ray(points[i], raycastCentre - points[i]);
            if (tempCollider.Raycast(ray, out hit, Vector3.Distance(raycastCentre,points[i]))) {
                pointsList.Add(hit.point + pointShift);
            }
            else {
                pointsMissed++;
            }

        }
        Debug.Log("casting complete, points missed: " + pointsMissed);
        points = pointsList.ToArray();
        DestroyImmediate(tempCollider);
        SaveFile();
    }

    Vector3 GetClosestCentre(Vector3 point) {
        Vector3 closestCentre = raycastCentres[0];
        float closestDist = Vector3.Distance(point, closestCentre);
        for (int i = 1; i < raycastCentres.Length; i++) {
            if (Vector3.Distance(point,raycastCentres[i]) < closestDist) {
                closestCentre = raycastCentres[i];
                closestDist = Vector3.Distance(point, raycastCentres[i]);
            }
        }
        return closestCentre;
    }

    private void SaveFile() {
        /*PointCloudData pd = new PointCloudData(points);
        cloudPrefab.points = points;
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(path + fileName, FileMode.Create, FileAccess.Write, FileShare.None);
        formatter.Serialize(stream, pd);
        stream.Close();*/
        if (cloudPrefab != null) {
            cloudPrefab.points = points;
        }
        else {
            Debug.LogError("No cloud prefab set, changes will not be saved");
        }
        
    }


    private void OnDrawGizmos() {
        if (showSphere) {
            Gizmos.color = Color.gray;
            Gizmos.DrawSphere(sphereOffset, sphereRadius);
        }
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
            foreach (Vector3 raycastCentre in raycastCentres) {
                Gizmos.DrawSphere(raycastCentre, 0.1f);
            }
        }
    }

}
