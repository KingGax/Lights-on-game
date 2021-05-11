using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LightsOn {
    namespace LightingSystem {

        public class BoidManager : MonoBehaviour {
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
            Material agentMat;
            [Header("Materials")]
            public ColouredMaterial materials;
            public Material redMat;
            public Material greenMat;
            public Material blueMat;
            // public Material hiddenRed;
            // public Material hiddenGreen;
            // public Material hiddenBlue;
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
            public LightColour col;
            bool visible = false;
            private float riseSpeed;
            private float risingYFloor;
            MeshRenderer renderer;
            public GameObject visionObject;
            private Transform followTransform;
            public bool followParent;
            bool doFollow = false;
            //Camera camera;
            List<AgentController> agents = new List<AgentController>();
            // Start is called before the first frame update
            private void Awake() {
                //camera = GameObject.Find("Main Camera").GetComponent<Camera>();
                renderer = visionObject.GetComponent<MeshRenderer>();
                destroyRoutine = DestroyAgents(1.5f);
                init = true;
                if (xMax < xMin) {
                    float tmp = xMax;
                    xMax = xMin;
                    xMin = tmp;
                }
                if (yMax < yMin) {
                    float tmp = yMax;
                    yMax = yMin;
                    yMin = tmp;
                }
                if (zMax < zMin) {
                    float tmp = zMax;
                    zMax = zMin;
                    zMin = tmp;
                }
                boidCentre = transform.position;
                //boidCentre = new Vector3(transform.position.x + Random.Range(xMin+0.01f, xMax), transform.position.y + Random.Range(yMin+0.01f, yMax), transform.position.z + Random.Range(zMin+0.01f, zMax));
                //boidCentre.y += 1f;
            }

            // public void SetCol(LightColour col){
            //     if (col == Color.red){
            //         agentMat = redMat;
            //     } else if (col == Color.green){
            //         agentMat = greenMat;
            //     } else{
            //         agentMat = blueMat;
            //     } 
            // }
            public void DirectAwayFromCentre(float disperseSpeed) {
                foreach (AgentController a in agents) {
                    a.DirectAwayFromCentre(disperseSpeed);
                }
            }
            public void MoveBoidCentre(Vector3 newCentre) {
                boidCentre = newCentre;
            }

            public void SetSpawnPoints(Vector3[] points, float maxRadius, Vector3 size, Vector3 pos) {
                spawnPoints = points;
                spawnPointsSet = true;
                agentCount = points.Length;
                maxRadiusSquare = Mathf.Pow(maxRadius, 2);
                //Vector3 newSize = new Vector3(size.x*transform.parent.localScale.x, size.y/transform.parent.localScale.y, size.z/transform.parent.localScale.z);
                visionObject.transform.position = boidCentre;
                visionObject.transform.localScale = size;
            }

            private GameObject SpawnBoid(Vector3 position) {
                GameObject newBoid = LocalObjectPool.SharedInstance.GetPooledBoid();
                if (newBoid != null) {
                    newBoid.transform.position = position;
                    newBoid.SetActive(true);
                }
                return newBoid;
            }

            public void StartRising(float _riseSpeed) {
                riseSpeed = _riseSpeed;
                boidCentre = GetAveragePos();
                risingYFloor = boidCentre.y;
                InvokeRepeating("UpdateRisingCentre",0.1f,0.1f);
            }

            private void UpdateRisingCentre() {
                boidCentre = GetAveragePos();
                risingYFloor += 0.1f * riseSpeed;
                boidCentre = new Vector3(boidCentre.x, Mathf.Max(risingYFloor, boidCentre.y), boidCentre.z);
            }

            public void Spawn() {
                //Related to issue: https://issuetracker.unity3d.com/issues/dot-material-changes-made-to-a-gameobject-also-apply-to-the-instantiated-gameobjects-material
                LineRenderer copyofmaterial = GetComponent<LineRenderer>(); //why do i have to do this :(
                copyofmaterial.material = new Material(materials.get(col));
                agentMat = copyofmaterial.material;
                //agentMat.color = new Color(agentMat.color.r, agentMat.color.g, agentMat.color.b, 1f);
                for (int i = 0; i < agentCount; i++) {
                    Vector3 pos;
                    if (spawnPointsSet) {
                        pos = spawnPoints[i];
                    } else {
                        pos = new Vector3(transform.position.x + Random.Range(xMin + 0.01f, xMax), transform.position.y + Random.Range(yMin + 0.01f, yMax), transform.position.z + Random.Range(zMin + 0.01f, zMax));
                    }
                    //GameObject g = Instantiate(agentPrefab, pos, new Quaternion(0, 0, 0, 0));
                    GameObject g = SpawnBoid(pos);
                    if (g != null) {
                        g.GetComponent<MeshRenderer>().material = agentMat;
                        g.transform.SetParent(transform);
                        AgentController a = g.GetComponent<AgentController>();
                        a.SetVals(xMin, xMax, yMin, yMax, zMin, zMax, agentSpeed, turnSpeed, detectionRadius, matchingRadius, centreBias, matchingBias, avoidanceBias, randomTurnAmount, maxRadiusSquare, 0.5f);
                        agents.Add(a);
                    }
                }
                checkOOBTimer = checkOOBTimerMax;
                updateTimer = updateTimerMax;
                StartCoroutine("Timers");
            }

            Vector3 GetAveragePos() {
                Vector3 acc = new Vector3(0, 0, 0);
                foreach (AgentController a in agents) {
                    acc += a.transform.position;
                }
                acc /= agents.Count;
                return acc;
            }

            public void SetFollowTransform(Transform _followTransform) {
                followTransform = _followTransform;
            }
            // Update is called once per frame
            void Update() {
                if (followParent) {
                    boidCentre = followTransform.position;
                    visionObject.transform.position = followTransform.position;
                }
                if (!visible && renderer.isVisible) {
                    visible = true;
                    foreach (AgentController agent in agents) {
                        agent.visible = true;
                    }
                } else if (visible && !renderer.isVisible) {
                    visible = false;
                    foreach (AgentController agent in agents) {
                        agent.visible = false;
                    }
                }
                checkOOBTimer -= Time.deltaTime;
                updateTimer -= Time.deltaTime;
                if (isReforming) {
                    reformTimer -= Time.deltaTime;
                    float lerp = (Mathf.Max(0, reformTimer) / reformTimerMax);
                    foreach (AgentController agent in agents) {
                        agent.LerpOpacity(lerp);
                    }

                }
            }

            public float SendReformSignal(Vector3[] newPoints) {
                spawnPoints = newPoints;
                reformTimer = reformTimerMax;
                //StartCoroutine(destroyRoutine);
                isReforming = true;
                if (newPoints != null) {
                    for (int i = 0; i < agents.Count; i++) {
                        agents[i].StartReform(spawnPoints[i]);
                    }
                } else {
                    for (int i = 0; i < agents.Count; i++) {
                        agents[i].StartReform(transform.position);
                    }
                }
                return reformTimerMax;
            }

            public void SetSpeeds(float speed, float turnSpeed) {
                foreach (AgentController a in agents) {
                    a.SetSpeed(speed, turnSpeed);
                }
            }

            public void CancelReform() {
                //StopCoroutine(destroyRoutine);
                isReforming = false;
                agentMat.color = new Color(agentMat.color.r, agentMat.color.g, agentMat.color.b, 1f);
                foreach (AgentController agent in agents) {
                    agent.StopReform(agentSpeed, turnSpeed);
                    agent.GetComponent<MeshRenderer>().material = agentMat;
                }

            }
            public void DestroyMyAgents() {
                for (int i = 0; i < transform.childCount; i++) {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
                transform.DetachChildren();
                Destroy(transform.parent.gameObject);
            }

            IEnumerator DestroyAgents(float time) {
                yield return new WaitForSeconds(time);
                if (isReforming) {
                    //Destroy(transform.parent.gameObject);
                    //DestroyMyAgents();
                }
            }

            IEnumerator Timers() {
                while (true) {
                    if (updateTimer <= 0) {
                        foreach (AgentController agent in agents) {
                            if (!agent.canUpdate) agent.canUpdate = true;
                        }
                        updateTimer = updateTimerMax;
                    }
                    if (checkOOBTimer <= 0) {
                        foreach (AgentController agent in agents) {
                            agent.canCheckOOB = true;
                        }
                        checkOOBTimer = checkOOBTimerMax;
                    }
                    if (isReforming && reformTimer <= 0) {
                        //lightableObject.FinishAppearing();
                        //Destroy(transform.parent.gameObject);
                        //DestroyMyAgents();
                    }
                    yield return null;
                }
            }

            private void OnDrawGizmos() {
                if (init) {
                    Gizmos.color = Color.green;
                    if (showBoundingBox) {
                        Gizmos.DrawWireCube(transform.position, new Vector3(xMax - xMin, yMax - yMin, zMax - zMin));
                    }
                }
            }
        }
    }
}
