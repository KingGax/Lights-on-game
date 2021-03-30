﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AgentController : MonoBehaviour
{
    float xMin;
    float xMax;
    float yMin;
    float yMax;
    float zMin;
    float zMax;
    float speed;
    float turnSpeed;
    BoidManager parent;
    Rigidbody rb;
    bool init = false;
    float detectionRadius;
    float matchingBias;
    float alignmentRadius;
    float centreBias;
    float matchingRadius;
    float avoidanceBias;
    Color gizmoCol;
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    public void SetVals(float xmin, float xmax, float ymin, float ymax, float zmin, float zmax, float _speed, float turnspeed, float detectionradius, float matchingradius,
    float centrebias, float matchingbias, float avoidancebias){
        xMin = xmin;
        xMax = xmax;
        yMin = ymin;
        yMax = ymax;
        zMin = zmin;
        zMax = zmax;
        speed = _speed;
        turnSpeed = turnspeed;
        detectionRadius = detectionradius;
        matchingRadius = matchingradius;
        rb = GetComponent<Rigidbody>();
        parent = GetComponentInParent<BoidManager>();
        init = true;
        matchingBias = matchingbias;
        centreBias = centrebias;
        avoidanceBias = avoidancebias;
        gizmoCol = Color.yellow;
    }
    // Update is called once per frame
    void Update()
    {
        if (init){
            //CheckOOB();
            
            
            Vector3 avoidanceVec = transform.forward; 
            bool avoiding = CheckCollisions(ref avoidanceVec);
            float avBias;
            Vector3 finalDir;
            if (avoiding){
                avBias = avoidanceBias;
                //finalDir = avoidanceVec;
            } else {
                avBias = 0; 
            }
            Vector3 centreDir = Vector3.RotateTowards(transform.forward, parent.boidCentre - transform.position, turnSpeed * Time.deltaTime, 0.0f);
            Vector3 matchingDir = MatchDirection(centreDir);
            finalDir = (centreDir * centreBias + matchingDir * matchingBias + avoidanceVec * avBias) / (centreBias+matchingBias+avBias);
            //finalDir = (centreDir * centreBias + avoidanceVec * avoidanceBias) / (centreBias+avoidanceBias);
            if(parent.showDirectionArrows){
                Debug.DrawRay(transform.position, finalDir, Color.red);
            }
            transform.rotation = Quaternion.LookRotation(finalDir);
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    Vector3 CombinedOverlap(Vector3 startDir){
        return startDir;
    }

    Vector3 MatchDirection(Vector3 startDir){
        Vector3 ret = startDir;
        Collider[] hits = Physics.OverlapSphere(transform.position, matchingRadius, ((1 << LayerMask.NameToLayer("SwarmLayer")))); // | (1 << LayerMask.NameToLayer("StaticEnvironment"))
        if (hits.Length > 0 && !(hits.Length == 1 && hits[0].transform == transform)){
            Vector3 acc = new Vector3(0,0,0);
            for (int i = 1; i < hits.Length; i++){
            //foreach(Collider hit in hits){
                Vector3 target = transform.position + hits[i].transform.forward;
                Vector3 newDirection = Vector3.RotateTowards(transform.forward, target, turnSpeed * Time.deltaTime, 0.0f);
                acc += newDirection;
            }
            acc /= (hits.Length-1);
            ret = acc;
        }
        return ret;
    }

    bool CheckCollisions(ref Vector3 dir){
        //return false;
        Vector3 acc = transform.forward;
        bool ret = false;
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, ((1 << LayerMask.NameToLayer("SwarmLayer"))));      
        if (hits.Length > 0 && !(hits.Length == 1 && hits[0].transform == transform)){
            ret = true;
            // Vector3 target = transform.position - hits[1].transform.position;
            // Vector3 newDirection = Vector3.RotateTowards(transform.forward, target, turnSpeed * Time.deltaTime, 0.0f);
            // acc = newDirection;
            for (int i = 1; i < hits.Length; i++){
            //foreach(Collider hit in hits){
                Vector3 target = transform.position - hits[i].transform.position;
                Vector3 newDirection = Vector3.RotateTowards(transform.forward, target, turnSpeed * Time.deltaTime, 0.0f);
                acc += newDirection-transform.forward;
            }
            acc /= hits.Length;
            dir = acc;
            //transform.rotation = Quaternion.LookRotation(acc);
        }
        if (parent.showAvoidanceHitboxes){
            if (ret){
                gizmoCol = Color.blue;
            } else {
                gizmoCol = Color.yellow;
            }
        }
        return ret;
    }

    float mod(float x, float m) {
        float r = x % m;
        return r < 0 ? r + m : r;
    }

    void CheckOOB(){
        //return;
        float x = mod((transform.position.x - xMin), (xMax - xMin)) + xMin;
        float y = mod((transform.position.y - yMin), (yMax - yMin)) + yMin;
        float z = mod((transform.position.z - zMin), (zMax - zMin)) + zMin;
        transform.position = new Vector3(x, y, z);
    }
    private void OnDrawGizmos() {
        if (init){
            Gizmos.color = gizmoCol;
            if (parent.showAvoidanceHitboxes){
                Gizmos.DrawWireSphere(transform.position, detectionRadius);
            }
            if (parent.showDirectionMatchingHitboxes){
                Gizmos.DrawWireSphere(transform.position, matchingRadius);
            }
        }
    }
}
