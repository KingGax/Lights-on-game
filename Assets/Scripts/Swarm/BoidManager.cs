﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    [Header("Box bounds")]
    public float xMin;
    public float xMax;
    public float yMin;
    public float yMax;
    public float zMin;
    public float zMax;
    [Header("Agent setup")]
    public int agentCount; //number of agents to spawn
    public float agentSpeed;
    public float turnSpeed; //degrees/second
    public float detectionRadius;
    public float matchingRadius;
    public Vector3 boidCentre;
    public GameObject agentPrefab;
    public Material agentMat;
    [Header("Materials")]
    public Material redMat;
    public Material greenMat;
    public Material blueMat;
    [Header("Biases")]
    public float centreBias;
    public float matchingBias;
    public float avoidanceBias;
    [Header("Debug")]
    public bool showDirectionArrows;
    public bool showAvoidanceHitboxes;
    public bool showDirectionMatchingHitboxes;
    public bool showBoundingBox;
    public float updateCentreTimerMax;
    float updateCentreTimer;
    bool init = false;
    //Camera camera;
    List<AgentController> agents = new List<AgentController>();
    // Start is called before the first frame update
     private void Awake() {
        //camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        
        init = true;
        if (xMax < xMin){
            float tmp = xMax;
            xMax = xMin;
            xMin = tmp;
        }
        if (yMax < yMin){
            float tmp = yMax;
            yMax = yMin;
            yMin = tmp;
        }
        if (zMax < zMin){
            float tmp = zMax;
            zMax = zMin;
            zMin = tmp;
        }
        //camera.transform.position = new Vector3(xMax, yMax, zMax);
        //camera.transform.LookAt(boidCentre);
        boidCentre = new Vector3(transform.position.x + Random.Range(xMin+0.01f, xMax), transform.position.y + Random.Range(yMin+0.01f, yMax), transform.position.z + Random.Range(zMin+0.01f, zMax));
        boidCentre.y += 1f;
        
    }
    
    public void SetMat(Color col){
        if (col == Color.red){
            agentMat = redMat;
        } else if (col == Color.green){
            agentMat = greenMat;
        } else{
            agentMat = blueMat;
        } 
    }

    public void Spawn(){
        for (int i = 0; i < agentCount; i++){
            Vector3 pos = new Vector3(transform.position.x + Random.Range(xMin+0.01f, xMax), transform.position.y + Random.Range(yMin+0.01f, yMax), transform.position.z + Random.Range(zMin+0.01f, zMax));
            GameObject g = Instantiate(agentPrefab, pos, new Quaternion(0,0,0,0));
            g.GetComponent<MeshRenderer>().material = agentMat;
            g.transform.SetParent(transform);
            AgentController a = g.GetComponent<AgentController>();
            a.SetVals(xMin, xMax, yMin, yMax, zMin, zMax, agentSpeed, turnSpeed, detectionRadius, matchingRadius, centreBias, matchingBias, avoidanceBias);
            agents.Add(a);
        }
        updateCentreTimer = 0f;
        StartCoroutine("Timers");
    }

    Vector3 GetAveragePos(){
        Vector3 acc = new Vector3(0, 0, 0);
        foreach (AgentController a in agents){
            acc += a.transform.position;
        }
        acc /= agents.Count;
        return acc;
    }

    // Update is called once per frame
    void Update()
    {
        //camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, Quaternion.LookRotation(boidCentre - camera.transform.position), Time.deltaTime);
        
        updateCentreTimer -= Time.deltaTime;
        //Physics.l
        //camera.transform.LookAt(boidCentre);
        //SmoothLook(boidCentre);
    }


    IEnumerator Timers(){
        while (true){
            if (updateCentreTimer <= 0){
                boidCentre = new Vector3(transform.position.x + Random.Range(xMin+0.01f, xMax), transform.position.y + Random.Range(yMin+0.01f, yMax) + 1f, transform.position.z + Random.Range(zMin+0.01f, zMax));
                //boidCentre = GetAveragePos();
                updateCentreTimer = updateCentreTimerMax;
            }
            yield return null;
        }
        
    }

    // void SmoothLook(Vector3 newDirection){
        
    // }
    private void OnDrawGizmos() {
        if (init){
            Gizmos.color = Color.green;
            if (showBoundingBox){
                Gizmos.DrawWireCube(transform.position, new Vector3(xMax - xMin, yMax - yMin, zMax - zMin));
            }
        }
    }
}
