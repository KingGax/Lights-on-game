using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LightsOn {
    namespace LightingSystem {

        public class LightableObject : MonoBehaviour {

            [SerializeField]
            protected LightColour colour;
            public ColouredMaterial materials;
            public ColouredMaterial hiddenMaterials;

            protected bool initialised = false;
            protected bool disappearOnStart = false;
            protected bool overrideMeshRenderer = false;

            float invisibleOpacity = 0.1f;
            protected LayerMask potentialColliders;
            bool isHidden = false;
            bool appearing = false;
            Bounds physicsBounds;
            float boundingSphereSize;
            protected int defaultLayer;
            int hiddenLayer;

            bool distCheckThisFrame = false;
            float lightAlwaysConsideredDist = 2f;
            float lightOverpowerRatio = 1.4f;

            List<Lanturn> currentLights = new List<Lanturn>();
            MeshRenderer meshRenderer;
            Collider physicsCollider;
            GameObject boidManagerPrefab;
            GameObject boidManagerInstance;
            protected bool canSwarm = false;
            public float maxSwarmRadius = 1;
            public PointCloudSO cloudPoints;
            Vector3[] transformedPoints;
            Quaternion lastRotation;
            bool fading = false;
            float fadeTimer = 0f;
            float fadeTimerMax = 0f;
            BoxCollider physicsBox;
            bool usesBoxCollider= false;
            
            //public Transform swarmPoint;
            virtual protected void Awake() {
                hiddenLayer = LayerMask.NameToLayer("HiddenObjects");
            }

            public virtual void Start() {
                canSwarm = true;
                boidManagerPrefab = GlobalValues.Instance.boidManagerPrefab;
                if (!overrideMeshRenderer) {
                    meshRenderer = transform.parent.GetComponent<MeshRenderer>();
                }
                
                physicsCollider = transform.parent.GetComponent<Collider>();
                physicsBox = GetComponentInParent<BoxCollider>();
                if (physicsBox != null) {
                    usesBoxCollider = true;
                }
                potentialColliders = GlobalValues.Instance.reappearPreventionLayers;
                defaultLayer = transform.parent.gameObject.layer;
                physicsBounds = physicsCollider.bounds;
                boundingSphereSize = Mathf.Max(physicsBounds.size.x, physicsBounds.size.y, physicsBounds.size.z);
                initialised = true;
                SetColour(colour);
                GetLightsInRange();
                ColourChanged();
            }

            private void FixedUpdate() {
                if (distCheckThisFrame) {
                    if (currentLights.Count > 0) {
                        if (ColourCheckWithDistance()) {
                            StartDisappear();
                        } else {
                            StartAppearing();
                        }
                    }
                }
                if (fading) {
                    fadeTimer -= Time.deltaTime;
                    float lerp = 1 - (Mathf.Max(0, fadeTimer) / fadeTimerMax);
                    LerpMaterial(lerp);
                    if (fadeTimer <= 0) {
                        fading = false;
                        FinishAppearing();
                    }
                }
                distCheckThisFrame = !distCheckThisFrame;
            }

            protected virtual void LerpMaterial(float lerp) {
                if (!overrideMeshRenderer) {
                    meshRenderer.material.Lerp(hiddenMaterials.get(colour), materials.get(colour), lerp);
                }
            }

            void GetLightsInRange() {
                for (int i = 0; i < GlobalValues.Instance.players.Count; i++) {
                    Lanturn currentLantern = GlobalValues.Instance.players[i].GetComponentInChildren<Lanturn>();
                    Collider lanternCol = currentLantern.gameObject.GetComponent<Collider>();
                    if (lanternCol != null) {
                        if (physicsCollider.bounds.Intersects(lanternCol.bounds)) {
                            if (!currentLights.Contains(currentLantern)) {
                                currentLights.Add(currentLantern);
                            }
                        }
                    } else {
                        Debug.LogError(currentLantern.gameObject);
                    }
                }
            }

            void StartAppearing() {
                if (isHidden && !appearing) {
                    appearing = true;
                    InvokeRepeating("TryAppear", 0f, 0.1f);
                }
            }

            void TryAppear() {
                if (CheckNoIntersections()) {
                    StartAppear();
                    CancelInvoke("TryAppear");
                    appearing = false;
                }
            }

            private bool ColourCheckWithDistance() {
                if (currentLights.Count == 0) {
                    return false;
                }

                float closestLight = float.MaxValue;
                foreach (Lanturn lo in currentLights) {
                    float dist = Vector3.Distance(transform.position, lo.gameObject.transform.position);
                    if (closestLight > dist) {
                        closestLight = dist;
                    }
                }

                LightColour lightColour = LightColour.Black;
                foreach (Lanturn lo in currentLights) {
                    float dist = Vector3.Distance(transform.position, lo.gameObject.transform.position);
                    if (dist < lightAlwaysConsideredDist || dist < lightOverpowerRatio * closestLight) {
                        lightColour = lightColour.MergeWith(lo.GetColour());
                    }
                }

                return lightColour == colour;
            }

            public void ColourChanged() {
                if (CheckColours(currentLights)) {
                    StartDisappear();
                } else {
                    StartAppearing();
                }
            }

            public virtual void SetColour(LightColour col) {
                colour = col;
                if (initialised && !overrideMeshRenderer) {
                    meshRenderer.material = materials.get(colour);
                }
            }

            public virtual bool CheckNoIntersections() {
                physicsBounds.center = transform.parent.position;

                Vector3 worldCenter = physicsCollider.bounds.center;

                Vector3 worldHalfExtents;
                if (usesBoxCollider) {
                    worldHalfExtents = new Vector3(physicsBox.size.x * transform.parent.localScale.x, physicsBox.size.y * transform.parent.localScale.y, physicsBox.size.z * transform.parent.localScale.z);
                    worldHalfExtents *= 0.5f;
                } else {
                    worldHalfExtents = physicsCollider.bounds.extents;
                }
                DrawCubePoints(CubePoints(worldCenter, worldHalfExtents, physicsCollider.transform.rotation));
                Collider[] closeColliders = Physics.OverlapBox(worldCenter, worldHalfExtents, physicsCollider.transform.rotation, potentialColliders);
                return closeColliders.Length == 0;
            }

            Vector3[] CubePoints(Vector3 center, Vector3 extents, Quaternion rotation) {
                Vector3[] points = new Vector3[8];
                points[0] = rotation * Vector3.Scale(extents, new Vector3(1, 1, 1)) + center;
                points[1] = rotation * Vector3.Scale(extents, new Vector3(1, 1, -1)) + center;
                points[2] = rotation * Vector3.Scale(extents, new Vector3(1, -1, 1)) + center;
                points[3] = rotation * Vector3.Scale(extents, new Vector3(1, -1, -1)) + center;
                points[4] = rotation * Vector3.Scale(extents, new Vector3(-1, 1, 1)) + center;
                points[5] = rotation * Vector3.Scale(extents, new Vector3(-1, 1, -1)) + center;
                points[6] = rotation * Vector3.Scale(extents, new Vector3(-1, -1, 1)) + center;
                points[7] = rotation * Vector3.Scale(extents, new Vector3(-1, -1, -1)) + center;

                return points;
            }

            void DrawCubePoints(Vector3[] points) {
                Debug.DrawLine(points[0], points[1], Color.red, 1f);
                Debug.DrawLine(points[0], points[2], Color.red, 1f);
                Debug.DrawLine(points[0], points[4], Color.red, 1f);

                Debug.DrawLine(points[7], points[6], Color.red, 1f);
                Debug.DrawLine(points[7], points[5], Color.red, 1f);
                Debug.DrawLine(points[7], points[3], Color.red, 1f);

                Debug.DrawLine(points[1], points[3], Color.red, 1f);
                Debug.DrawLine(points[1], points[5], Color.red, 1f);
                Debug.DrawLine(points[2], points[3], Color.red, 1f);
                Debug.DrawLine(points[2], points[6], Color.red, 1f);

                Debug.DrawLine(points[4], points[5], Color.red, 1f);
                Debug.DrawLine(points[4], points[6], Color.red, 1f);
            }

            //Returns true if colours match - only deals with one colour currently
            bool CheckColours(List<Lanturn> lights) {
                if (lights.Count == 0) {
                    return false;
                }

                LightColour lightColour = LightColour.Black;
                for (int i = 1; i < lights.Count; i++) {
                    lightColour = lightColour.MergeWith(lights[i].GetColour());
                }

                return lightColour == colour;
            }

            void OnTriggerEnter(Collider other) {
                Lanturn newLight = other.GetComponent<Lanturn>();
                if (newLight != null) {
                    if (!currentLights.Contains(newLight)) {
                        currentLights.Add(newLight);
                    }

                    if (CheckColours(currentLights)) {
                        StartDisappear();
                    } else {
                        StartAppearing();
                    }
                }
            }
            private Vector3[] GetTransformedPoints() {
                if (cloudPoints != null) {
                    Quaternion defaultQuat = cloudPoints.initialRotation;
                    Quaternion rotationQuat = transform.parent.rotation * Quaternion.Inverse(defaultQuat); //trivially
                    Vector3[] newPoints = new Vector3[cloudPoints.points.Length];
                    for (int i = 0; i < cloudPoints.points.Length; i++) {
                        newPoints[i] = transform.parent.position + rotationQuat * cloudPoints.points[i];
                    }
                    return newPoints;
                } else {
                    return null;
                }
            }


            public virtual void Disappear() {
                if (!overrideMeshRenderer) {
                    meshRenderer.material = hiddenMaterials.get(colour);
                    meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
                transform.parent.gameObject.layer = hiddenLayer;
                if (canSwarm) {
                    fading = false;
                    if (boidManagerInstance == null) {
                        boidManagerInstance = Instantiate(boidManagerPrefab, transform.position, transform.rotation);
                        //boidManagerInstance.transform.parent = transform.parent;
                        BoidManager man = boidManagerInstance.GetComponentInChildren<BoidManager>();
                        man.boidCentre = transform.TransformPoint(GetComponent<BoxCollider>().center);
                        //man.lightableObject = this;
                        man.col = colour;
                        //man.SetCol(colour);
                        if (cloudPoints != null) {
                            man.SetSpawnPoints(GetTransformedPoints(), maxSwarmRadius, GetComponent<BoxCollider>().bounds.size, GetComponent<BoxCollider>().transform.position);
                        }
                        man.Spawn();
                    } else {
                        BoidManager man = boidManagerInstance.GetComponentInChildren<BoidManager>();
                        man.CancelReform();
                    }
                }

                Light[] lights = GetComponentsInChildren<Light>();
                foreach (Light l in lights) {
                    l.enabled = false;
                }
            }

            public virtual void Appear() {
                if (!overrideMeshRenderer) {
                    meshRenderer.material = materials.get(colour);
                    meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }
                transform.parent.gameObject.layer = defaultLayer;
                if (canSwarm) {
                    fadeTimerMax = boidManagerInstance.GetComponentInChildren<BoidManager>().SendReformSignal(GetTransformedPoints());
                    fadeTimer = fadeTimerMax;
                    fading = true;
                    //Destroy(boidManagerInstance);
                }
                //move for optimisation at some point
                Light[] lights = GetComponentsInChildren<Light>();
                foreach (Light l in lights) {
                    l.enabled = true;
                }
            }

            void StartDisappear() {
                if (initialised) {
                    if (!isHidden) {
                        isHidden = true;
                        appearing = false;
                        CancelInvoke("TryAppear");
                        Disappear();
                    }

                    if (appearing) {
                        appearing = false;
                        CancelInvoke("TryAppear");
                    }
                } else {
                    disappearOnStart = true;
                }

            }
            void StartAppear() {
                isHidden = false;
                Appear();
            }

            public virtual void FinishAppearing() {
                if (!overrideMeshRenderer) {
                    meshRenderer.material = materials.get(colour);
                }
            }

            void OnTriggerExit(Collider other) {
                Lanturn newLight = other.GetComponent<Lanturn>();
                if (newLight != null) {
                    currentLights.Remove(newLight);
                    if (CheckColours(currentLights)) {
                        StartDisappear();
                    } else {
                        StartAppearing();
                    }
                }
            }

            private void OnDrawGizmos() {
                if (initialised) {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireCube(physicsCollider.bounds.center, physicsCollider.bounds.size);
                    Gizmos.DrawWireSphere(physicsCollider.bounds.center, boundingSphereSize);
                }
            }
        }
    }
}