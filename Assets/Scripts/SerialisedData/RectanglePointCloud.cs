using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectanglePointCloud : PointCloud
{
    public float rectWidth;
    public float rectDepth;
    public float yNumSteps;
    public float widthSamples;
    public float depthSamples;
    public Vector3 bottomCentre;
    public Vector3 topCentre;
    public bool showRaycastCentre;
    public bool showRaycasts;

    protected override Vector3[] CreatePointList() {
        Mesh colliderMesh = new Mesh();
        int pointsMissed = 0;
        MeshCollider tempCollider = gameObject.AddComponent<MeshCollider>();
        if (mesh == null) {
            Debug.LogError("No mesh assigned");
        }
        tempCollider.sharedMesh = mesh;
        List<Vector3> pointsList = new List<Vector3>();

        for (int i = 0; i < yNumSteps; i++) {
            for (int j = 0; j < widthSamples; j++) {
                RaycastHit hit;
                Vector3 centrePoint = Vector3.Lerp(bottomCentre, topCentre, i / yNumSteps);
                Vector3 rayMiddle = Vector3.Lerp(centrePoint - Vector3.forward * rectDepth, centrePoint + Vector3.forward * rectDepth, j / (widthSamples-1));
                Vector3 frontRayStart = centrePoint + Vector3.right * rectWidth;
                Vector3 backRayStart = centrePoint - Vector3.right * rectWidth;
                Ray frontRay = new Ray(frontRayStart, rayMiddle - frontRayStart);
                if (tempCollider.Raycast(frontRay, out hit, (rayMiddle - frontRayStart).magnitude)) {
                    pointsList.Add(hit.point + pointShift);
                }
                else {
                    pointsMissed++;
                }
                Ray backRay = new Ray(backRayStart, rayMiddle - backRayStart);
                if (tempCollider.Raycast(backRay, out hit, (rayMiddle - backRayStart).magnitude)) {
                    pointsList.Add(hit.point + pointShift);
                }
                else {
                    pointsMissed++;
                }
            }
            for (int k = 0; k < depthSamples; k++) {
                RaycastHit hit;
                Vector3 centrePoint = Vector3.Lerp(bottomCentre, topCentre, i / yNumSteps);
                Vector3 rayMiddle = Vector3.Lerp(centrePoint - Vector3.left * rectWidth, centrePoint + Vector3.left * rectWidth, k / (depthSamples - 1));
                Vector3 frontRayStart = rayMiddle + Vector3.forward * rectDepth;
                Vector3 backRayStart = rayMiddle - Vector3.forward * rectDepth;
                Ray frontRay = new Ray(frontRayStart, rayMiddle - frontRayStart);
                if (tempCollider.Raycast(frontRay, out hit, (rayMiddle - frontRayStart).magnitude)) {
                    pointsList.Add(hit.point + pointShift);
                }
                else {
                    pointsMissed++;
                }
                Ray backRay = new Ray(backRayStart, rayMiddle - backRayStart);
                if (tempCollider.Raycast(backRay, out hit, (rayMiddle - backRayStart).magnitude)) {
                    pointsList.Add(hit.point + pointShift);
                }
                else {
                    pointsMissed++;
                }
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
                Gizmos.DrawSphere(bottomCentre, 0.1f);
                Gizmos.DrawSphere(topCentre, 0.1f);
                Gizmos.DrawLine(bottomCentre, bottomCentre + Vector3.forward * rectDepth);
                Gizmos.DrawLine(bottomCentre, bottomCentre - Vector3.forward * rectDepth);
                Gizmos.DrawLine(bottomCentre, bottomCentre + Vector3.right * rectWidth);
                Gizmos.DrawLine(bottomCentre, bottomCentre - Vector3.right * rectWidth);
            }
            if (showRaycasts) {
                for (int i = 0; i < yNumSteps; i++) {
                    for (int j = 0; j < widthSamples; j++) {
                        RaycastHit hit;
                        Vector3 centrePoint = Vector3.Lerp(bottomCentre, topCentre, i / yNumSteps);
                        Vector3 rayMiddle = Vector3.Lerp(centrePoint - Vector3.forward * rectDepth, centrePoint + Vector3.forward * rectDepth, j / (widthSamples - 1));
                        Vector3 frontRayStart = rayMiddle + Vector3.right * rectWidth;
                        Vector3 backRayStart = rayMiddle - Vector3.right * rectWidth;
                        Ray frontRay = new Ray(frontRayStart, rayMiddle - frontRayStart);
                        Gizmos.DrawRay(frontRay);
                        Ray backRay = new Ray(backRayStart, rayMiddle - backRayStart);
                        Gizmos.DrawRay(backRay);
                    }
                    for (int k = 0; k < depthSamples; k++) {
                        RaycastHit hit;
                        Vector3 centrePoint = Vector3.Lerp(bottomCentre, topCentre, i / yNumSteps);
                        Vector3 rayMiddle = Vector3.Lerp(centrePoint - Vector3.left * rectWidth, centrePoint + Vector3.left * rectWidth, k / (depthSamples - 1));
                        Vector3 frontRayStart = rayMiddle + Vector3.forward * rectDepth;
                        Vector3 backRayStart = rayMiddle - Vector3.forward * rectDepth;
                        Ray frontRay = new Ray(frontRayStart, rayMiddle - frontRayStart);
                        Gizmos.DrawRay(frontRay);
                        Ray backRay = new Ray(backRayStart, rayMiddle - backRayStart);
                        Gizmos.DrawRay(backRay);
                    }
                }
            }
            
        }
    }
}
