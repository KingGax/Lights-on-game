using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LightsOn{
namespace LightingSystem{

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
    public float randomTurnAmount; //rad/s
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
    float maxRadiusSquare; //max agent radius
    public float checkOOBTimerMax;
    float checkOOBTimer;
    public float updateTimerMax = 0.100f;
    float updateTimer;
    public float reformTimerMax = 1.2f;
    float reformTimer;
    bool init = false;
    private Vector3[] spawnPoints;
    private bool spawnPointsSet = false;
    bool isReforming = false;
    private IEnumerator destroyRoutine;
    //Camera camera;
    List<AgentController> agents = new List<AgentController>();
    public LightableObject lightableObject;
    // Start is called before the first frame update
     private void Awake() {
        //camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        
        destroyRoutine = DestroyAgents(1.5f);
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
        boidCentre = transform.position;
        //boidCentre = new Vector3(transform.position.x + Random.Range(xMin+0.01f, xMax), transform.position.y + Random.Range(yMin+0.01f, yMax), transform.position.z + Random.Range(zMin+0.01f, zMax));
        //boidCentre.y += 1f;
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

    public void SetSpawnPoints(Vector3[] points, float maxRadius) {
        spawnPoints = points;
        spawnPointsSet = true;
        agentCount = points.Length;
        maxRadiusSquare = Mathf.Pow(maxRadius, 2);
    }

    public void Spawn(){
        for (int i = 0; i < agentCount; i++){
            Vector3 pos;
            if (spawnPointsSet) {
                pos = spawnPoints[i];
            }
            else {
                pos = new Vector3(transform.position.x + Random.Range(xMin + 0.01f, xMax), transform.position.y + Random.Range(yMin + 0.01f, yMax), transform.position.z + Random.Range(zMin + 0.01f, zMax));
            }
            GameObject g = Instantiate(agentPrefab, pos, new Quaternion(0,0,0,0));
            g.GetComponent<MeshRenderer>().material = agentMat;
            g.transform.SetParent(transform);
            AgentController a = g.GetComponent<AgentController>();
            a.SetVals(xMin, xMax, yMin, yMax, zMin, zMax, agentSpeed, turnSpeed, detectionRadius, matchingRadius, centreBias, matchingBias, avoidanceBias, randomTurnAmount, maxRadiusSquare, 0.5f);
            agents.Add(a);
        }
        checkOOBTimer = checkOOBTimerMax;
        updateTimer = updateTimerMax;
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
        checkOOBTimer -= Time.deltaTime;
        updateTimer -= Time.deltaTime;
        if (isReforming) reformTimer -= Time.deltaTime;
    }

    public float SendReformSignal(){
        reformTimer = reformTimerMax;
        //StartCoroutine(destroyRoutine);
        isReforming = true;
        foreach(AgentController agent in agents){
            agent.StartReform();
        } 
        return reformTimerMax;
    }

    public void CancelReform(){
        //StopCoroutine(destroyRoutine);
        isReforming = false;
        foreach(AgentController agent in agents){
            agent.StopReform(agentSpeed, turnSpeed);
        }
    }

    IEnumerator DestroyAgents(float time){
        yield return new WaitForSeconds(time);
        if (isReforming){
            Destroy(gameObject);
        }
    }

    IEnumerator Timers(){
        while (true){
           if (updateTimer <= 0){
               foreach(AgentController agent in agents){
                    if (!agent.canUpdate) agent.canUpdate = true;
                }
                updateTimer = updateTimerMax;
            }
            if (checkOOBTimer <= 0){
                foreach(AgentController agent in agents){
                    agent.canCheckOOB = true;
                }
                checkOOBTimer = checkOOBTimerMax;
            }
            if (isReforming && reformTimer <= 0){
                //lightableObject.FinishAppearing();
                Destroy(gameObject);
            }
            yield return null;
        }
    }

    private void OnDrawGizmos() {
        if (init){
            Gizmos.color = Color.green;
            if (showBoundingBox){
                Gizmos.DrawWireCube(transform.position, new Vector3(xMax - xMin, yMax - yMin, zMax - zMin));
            }
        }
    }
}}}
