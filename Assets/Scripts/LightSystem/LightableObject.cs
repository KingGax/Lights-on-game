using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace LightsOn.LightingSystem {
    public class LightableObject : MonoBehaviour {

        [SerializeField]
        protected LightColour colour;
        public ColouredMaterial materials;
        public ColouredMaterial hiddenMaterials;
        public bool showReappearParticles;
        public Transform particleTransform;

        protected bool initialised = false; //this is used to make sure lightable object start has been called before other functions set materials
        protected bool disappearOnStart = false;
        protected bool overrideMeshRenderer = false; //this was used to allow lightable objects to have SkinnedMeshRenderers or MeshRenderers throughout development whilst waiting for models

        protected LayerMask potentialColliders;
        bool isHidden = false;
        bool appearing = false;
        Bounds physicsBounds;
        float boundingSphereSize;
        protected int defaultLayer;
        int hiddenLayer;

        bool distCheckThisFrame = false; 
        float lightAlwaysConsideredDist = 2f; //distance where a light is counted regardless of distance ratio
        float lightOverpowerRatio = 1.4f; //ratio of distances where lights can overpower 

        List<Lanturn> currentLights = new List<Lanturn>(); //List of lights in range
        protected MeshRenderer meshRenderer; 
        Collider physicsCollider;
        GameObject boidManagerPrefab;
        GameObject deathBoidManagerPrefab;
        GameObject boidManagerInstance;
        public bool canSwarm = true;
        public float maxSwarmRadius = 1;
        public PointCloudSO cloudPoints;

        protected bool fading = false;
        float fadeTimer = 0f;
        float fadeTimerMax = 0f; //this is the time taken to reform
        BoxCollider physicsBox;
        bool usesBoxCollider= false; //allows for other colliders than boxes
        bool extraSwarmSpawnPoints = false; //used for adding child object parts to swarm
        Vector3[] extraSwarmPoints; //used for adding child object parts to swarm
        
        //public Transform swarmPoint;
        virtual protected void Awake() {
            hiddenLayer = LayerMask.NameToLayer("HiddenObjects");
        }

        public LightColour GetColour() {
            return colour;
        }

        public virtual void Start() {
            boidManagerPrefab = GlobalValues.Instance.boidManagerPrefab;
            deathBoidManagerPrefab = GlobalValues.Instance.boidDeathPrefab;
            if (!overrideMeshRenderer) {
                meshRenderer = transform.parent.GetComponent<MeshRenderer>();
            }
            
            physicsCollider = transform.parent.GetComponent<Collider>();
            physicsBox = transform.parent.GetComponent<BoxCollider>();
            if (physicsBox != null) { //check for box collider to do better bounds checking when reappearing
                usesBoxCollider = true;
            }
            potentialColliders = GlobalValues.Instance.reappearPreventionLayers; 
            defaultLayer = transform.parent.gameObject.layer;
            physicsBounds = physicsCollider.bounds;
            boundingSphereSize = Mathf.Max(physicsBounds.size.x, physicsBounds.size.y, physicsBounds.size.z);
            initialised = true;
            SetColour(colour);
            GetLightsInRange();
        }

        private void FixedUpdate() {
            if (distCheckThisFrame) {
                if (currentLights.Count > 0) {
                    if (ColourCheckWithDistance()) { //This allows things to appear and disappear as lights move away from them
                        StartDisappear();
                    } else {
                        StartAppearing();
                    }
                }
            }
            if (fading) {//this is the material lerp
                fadeTimer -= Time.deltaTime;
                float lerp = 1 - (Mathf.Max(0, fadeTimer) / fadeTimerMax);
                LerpMaterial(lerp);
                if (fadeTimer <= 0) {
                    fading = false;
                    FinishAppearing();
                    CancelInvoke("TryReform");
                }
            }
            distCheckThisFrame = !distCheckThisFrame;//only check every other physics step for efficiency
        }

        //This function interpolates between the hidden and visible materials  
        protected virtual void LerpMaterial(float lerp) {
            if (!overrideMeshRenderer) {
                //meshRenderer.material.Lerp(hiddenMaterials.get(colour), materials.get(colour), lerp);
                Color tempcolor = meshRenderer.material.color;
                tempcolor.a = Mathf.Lerp(hiddenMaterials.get(colour).color.a, materials.get(colour).color.a, lerp);
                meshRenderer.material.color = tempcolor;
            }
        }

        //This function gets all player lights in range, used on start for if a lightable object is spawned inside a players range of influence
        void GetLightsInRange() {
            //for all players
            for (int i = 0; i < GlobalValues.Instance.players.Count; i++) {
                Lanturn currentLantern = GlobalValues.Instance.players[i].GetComponentInChildren<Lanturn>();
                Collider lanternCol = currentLantern.gameObject.GetComponent<Collider>();
                //check if within the bounds of the lanterns hitbox
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
        //This function starts the process of trying to reappers so invokes checking for overlapping objects
        void StartAppearing() {
            if (isHidden && !appearing) {
                appearing = true;
                InvokeRepeating("TryAppear", 0f, 0.1f);
            }
        }

        //this function starts an appear operation if there are no overlapping objects 
        protected virtual void TryAppear() {
            if (CheckNoIntersections()) {
                StartAppear();
                CancelInvoke("TryAppear");
                appearing = false;
            }
        }

        void TryReform() {
            if (!CheckNoIntersections()) {
                fading = false;
                StartDisappear();
                //BoidManager man = boidManagerInstance.GetComponentInChildren<BoidManager>();
                //man.CancelReform();
                CancelInvoke("TryReform");
            }
        }

        //This function returns true if the object should be hidden based off of the current lights and distances between them
        private bool ColourCheckWithDistance() {
            if (currentLights.Count == 0) {
                return false;
            }

            //find the closest light distance
            float closestLight = float.MaxValue;
            foreach (Lanturn lo in currentLights) {
                float dist = Vector3.Distance(transform.position, lo.gameObject.transform.position);
                if (closestLight > dist) {
                    closestLight = dist;
                }
            }

            //only count lights that are minimum distance or aren't significantly further than the closest light
            LightColour lightColour = LightColour.Black;
            foreach (Lanturn lo in currentLights) {
                float dist = Vector3.Distance(transform.position, lo.gameObject.transform.position);
                if (dist < lightAlwaysConsideredDist || dist < lightOverpowerRatio * closestLight) {
                    lightColour = lightColour.MergeWith(lo.GetColour());
                }
            }

            return lightColour == colour;
        }

        //This method is called when a light colour changes to allow the objects to update
        public void ColourChanged() {
            if (ColourCheckWithDistance()) {
                StartDisappear();
            } else {
                StartAppearing();
            }
        }

        //This is used to set the colour of an object and will update the colour visuals
        public virtual void SetColour(LightColour col) {
            colour = col;
            if (initialised && !overrideMeshRenderer) {
                meshRenderer.material = materials.get(colour);
            }
        }

        //This checks for overlapping objects to prevent objects appearing inside of each other
        public virtual bool CheckNoIntersections() {
            physicsBounds.center = transform.parent.position;

            Vector3 worldCenter = physicsCollider.bounds.center;

            Vector3 worldHalfExtents;
            if (usesBoxCollider) {//perfect size check for box colliders
                worldHalfExtents = new Vector3(physicsBox.size.x * transform.parent.localScale.x, physicsBox.size.y * transform.parent.localScale.y, physicsBox.size.z * transform.parent.localScale.z);
                worldHalfExtents *= 0.5f;
            } else {//larger generic check
                worldHalfExtents = physicsCollider.bounds.extents;
            }
            //DrawCubePoints(CubePoints(worldCenter, worldHalfExtents, physicsCollider.transform.rotation));
            Collider[] closeColliders = Physics.OverlapBox(worldCenter, worldHalfExtents, physicsCollider.transform.rotation, potentialColliders);
            return closeColliders.Length == 0;
        }

        //useful function for deabugging reappear boxes
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

        //useful function for deabugging reappear boxes
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

        //Returns true if colours match and object should disappear
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

        //On trigger used to handle lights entering within range 
        void OnTriggerEnter(Collider other) {
            Lanturn newLight = other.GetComponent<Lanturn>();
            if (newLight != null) {
                if (!currentLights.Contains(newLight)) {
                    currentLights.Add(newLight);
                }
                if (initialised) {
                    if (CheckColours(currentLights)) {
                        StartDisappear();
                    } else {
                        StartAppearing();
                    }
                }
            }
        }

        //Used for rotating point clouds when their objects rotate
        protected Vector3[] GetTransformedPoints() {
            if (cloudPoints != null) {
                Quaternion defaultQuat = cloudPoints.initialRotation;
                Quaternion rotationQuat = transform.parent.rotation * Quaternion.Inverse(defaultQuat); //calculates the rotation to apply to the points
                Vector3[] newPoints = new Vector3[cloudPoints.points.Length];
                //transforms each point to be rotated correctly
                for (int i = 0; i < cloudPoints.points.Length; i++) {
                    newPoints[i] = transform.parent.position + rotationQuat * cloudPoints.points[i];
                }
                return newPoints;
            } else {
                return null;
            }
        }

        //Used for rotating arbitrary point clouds about a transform, useful for rotating child object transforms
        protected Vector3[] GetTransformedPoints(PointCloudSO pointCloud,Transform rotTransform) {
            if (pointCloud != null) {
                Quaternion defaultQuat = pointCloud.initialRotation;
                Quaternion rotationQuat = rotTransform.rotation * Quaternion.Inverse(defaultQuat); //trivially
                Vector3[] newPoints = new Vector3[pointCloud.points.Length];
                for (int i = 0; i < pointCloud.points.Length; i++) {
                    newPoints[i] = rotTransform.position + rotationQuat * pointCloud.points[i];
                }
                return newPoints;
            } else {
                return null;
            }
        }

        protected BoidManager GetCurrentBoidManagerInstance() {
            return boidManagerInstance.GetComponentInChildren<BoidManager>();
        }
        
        //Creates a boid manager that handles a death cloud
        protected void SpawnDeathCloud(Vector3[] points, LightColour col) {
            boidManagerInstance = Instantiate(deathBoidManagerPrefab, transform.position, transform.rotation);
            //boidManagerInstance.transform.parent = transform.parent;
            BoidManager man = boidManagerInstance.GetComponentInChildren<BoidManager>();
            man.boidCentre = transform.TransformPoint(GetComponent<BoxCollider>().center);
            //man.lightableObject = this;
            man.col = col;
            //man.SetCol(colour);
            if (cloudPoints != null) {
                man.SetSpawnPoints(points, maxSwarmRadius, GetComponent<BoxCollider>().bounds.size, GetComponent<BoxCollider>().transform.position);
            }
            man.Spawn();
        }

        //Creates a boid manager that handles a death cloud
        public void SpawnDeathCloud() {
            if (canSwarm) {
                SpawnDeathCloud(GetTransformedPoints(),colour);
            }
        }

        //The method that handles objects disappearing and can be overwritten for different behaviour
        public virtual void Disappear() {
            if (!overrideMeshRenderer) {
                meshRenderer.material = hiddenMaterials.get(colour);
                meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            SwarmDisappearLogic();
        }
        
        //This method handles creating swarms on disappear
        protected void SwarmDisappearLogic(){ //to fix inheritance/overriding issue
            transform.parent.gameObject.layer = hiddenLayer;
            if (canSwarm) {
                fading = false;
                if (boidManagerInstance == null) {//if no swarm currently
                    //setup boid manager
                    boidManagerInstance = Instantiate(boidManagerPrefab, transform.position, transform.rotation);
                    BoidManager man = boidManagerInstance.GetComponentInChildren<BoidManager>();
                    man.boidCentre = transform.TransformPoint(GetComponent<BoxCollider>().center);
                    man.col = colour;
                    //set spawn points
                    if (cloudPoints != null) {
                        //handle children adding extra points e.g. weapons that rotate seperately to the enemy
                        if (!extraSwarmSpawnPoints) {
                            man.SetSpawnPoints(GetTransformedPoints(), maxSwarmRadius, GetComponent<BoxCollider>().bounds.size, GetComponent<BoxCollider>().transform.position);
                        } else {
                            Vector3[] allSpawnPoints = GetTransformedPoints().Concat(extraSwarmPoints).ToArray();
                            man.SetSpawnPoints(allSpawnPoints, maxSwarmRadius, GetComponent<BoxCollider>().bounds.size, GetComponent<BoxCollider>().transform.position);
                        }
                        

                    }
                    man.Spawn();
                } else {//finds existing and stops it reforming
                    BoidManager man = boidManagerInstance.GetComponentInChildren<BoidManager>();
                    man.CancelReform();
                }
            }

            //disable visual lights
            Light[] lights = GetComponentsInChildren<Light>();
            foreach (Light l in lights) {
                l.enabled = false;
            }
        }

        //Method to allow child objects to add more points to swarm cloud
        protected void SetExtraSpawnPoints(Vector3[] spawnPoints) {
            extraSwarmSpawnPoints = true;
            extraSwarmPoints = spawnPoints;
        }

        //Base appear method that is called when reappearing and can be overrided
        public virtual void Appear() {
            if (!overrideMeshRenderer) {
                //meshRenderer.material = materials.get(colour);
                meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                //LerpMaterial(0); why?
            }
            if (canSwarm) {
                if (!extraSwarmSpawnPoints) {//handling extra swarm points
                    fadeTimerMax = boidManagerInstance.GetComponentInChildren<BoidManager>().SendReformSignal(GetTransformedPoints());
                } else {
                    fadeTimerMax = boidManagerInstance.GetComponentInChildren<BoidManager>().SendReformSignal(GetTransformedPoints().Concat(extraSwarmPoints).ToArray());
                }
                
                fadeTimer = fadeTimerMax;
                fading = true;
                InvokeRepeating("TryReform", 0, 0.1f);
                //Destroy(boidManagerInstance);
            } else {
                FinishAppearing();
            }
            //enable visual lights
            Light[] lights = GetComponentsInChildren<Light>();
            foreach (Light l in lights) {
                l.enabled = true;
            }
        }

        //Method to make an object disappear regardless of lighting
        public virtual void ForceDisappear() {
            StartDisappear();
            Invoke("DelayedColourCheck", 0.1f);
        }

        //Method with no parameters to allow invokation 
        private void DelayedColourCheck() {
            if (!CheckColours(currentLights)) {
                StartAppearing();
            }
        }

        //This method is used to handle the internal lightable object variables when they disappear. It decides when to call disappear
        void StartDisappear() {
            if (initialised) {//prevents objects disappearing before they have their materials
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
        //Method to handle internal state then call Appear
        void StartAppear() {
            isHidden = false;
            Appear();
        }

        //Method for when objects commit to appearing and completely solidifies
        public virtual void FinishAppearing() {
            if (!overrideMeshRenderer) {
                meshRenderer.material = materials.get(colour);
                if (showReappearParticles){
                    //particle effect at ParticleTransform
                }
            }
            FinishAppearingSwarm();
            
        }

        //Method for handling disabling swarm agents when the object reappears
        void FinishAppearingSwarm(){
            transform.parent.gameObject.layer = defaultLayer;
            if (canSwarm) {
                boidManagerInstance.GetComponentInChildren<BoidManager>().DestroyMyAgents();
            }
        }

        //Unity Trigger method for removing lights when they move out of range
        void OnTriggerExit(Collider other) {
            Lanturn newLight = other.GetComponent<Lanturn>();
            if (newLight != null) {
                currentLights.Remove(newLight);
                if (initialised) {
                    if (CheckColours(currentLights)) {
                        StartDisappear();
                    } else {
                        StartAppearing();
                    }
                }
            }
        }

        //Debugging gizmo methods - only called in the editor when gizmos are on
        private void OnDrawGizmos() {
            if (initialised) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(physicsCollider.bounds.center, physicsCollider.bounds.size);
                Gizmos.DrawWireSphere(physicsCollider.bounds.center, boundingSphereSize);
            }
        }
    }
}