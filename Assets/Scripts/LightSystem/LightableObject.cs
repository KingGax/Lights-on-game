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
    protected bool canSwarm;
    public float maxSwarmRadius = 1;
    public PointCloudSO cloudPoints;
    Vector3[] transformedPoints;
    Quaternion lastRotation;
    bool fading = false;
    float fadeTimer = 0f;
    float fadeTimerMax = 0f;

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
                }
                else {
                    StartAppearing();
                }
            }
        }
        if (fading){
            fadeTimer -= Time.deltaTime;
            float lerp = 1-(Mathf.Max(0, fadeTimer)/fadeTimerMax);
            LerpMaterial(lerp);
            if (fadeTimer <= 0){
                fading = false;
                FinishAppearing();
            }
        }
        distCheckThisFrame = !distCheckThisFrame;
    }

    protected virtual void LerpMaterial(float lerp){
        if (!overrideMeshRenderer){
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
            }
            else {
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
        Collider[] closeColliders = Physics.OverlapSphere(physicsCollider.bounds.center, boundingSphereSize, potentialColliders);
        foreach (Collider col in closeColliders) {
            if (physicsCollider.bounds.Intersects(col.bounds)) {
                return false;
            }
        }
        return true;
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
            }
            else {
                StartAppearing();
            }
        }
    }
    private Vector3[] GetTransformedPoints() {
        bool regeneratePoints = false;
        if (transformedPoints == null) {
            regeneratePoints = true;
        } else if (transform.localRotation != lastRotation) {
            regeneratePoints = true;
        }
        if (regeneratePoints) {
            Quaternion defaultQuat = cloudPoints.initialRotation;
            Quaternion rotationQuat = transform.parent.rotation * Quaternion.Inverse(defaultQuat); //trivially
            Vector3[] newPoints = new Vector3[cloudPoints.points.Length];
            for (int i = 0; i < cloudPoints.points.Length; i++) {
                newPoints[i] = transform.parent.position + rotationQuat * cloudPoints.points[i];
            }
            return newPoints;
        } else {
            return transformedPoints;
        }
    }


    public virtual void Disappear() {
        if (!overrideMeshRenderer) {
            meshRenderer.material = hiddenMaterials.get(colour);
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        transform.parent.gameObject.layer = hiddenLayer;
        if (canSwarm){
            fading = false;
            if (boidManagerInstance == null){
                boidManagerInstance = Instantiate(boidManagerPrefab, transform.position, transform.rotation);
                boidManagerInstance.transform.parent = transform.parent;
                BoidManager man = boidManagerInstance.GetComponent<BoidManager>();
                man.lightableObject = this;
                man.SetMat(colour.ToColor());
                if (cloudPoints != null) {
                    man.SetSpawnPoints(GetTransformedPoints(), maxSwarmRadius);
                }
                man.Spawn();
            } else {
                BoidManager man = boidManagerInstance.GetComponent<BoidManager>();
                man.CancelReform();
            }
        }
        Tooltip[] tooltips = GetComponentsInChildren<Tooltip>();
        foreach (Tooltip t in tooltips) {
            t.Dismiss();
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
        if (canSwarm){
            fadeTimerMax = boidManagerInstance.GetComponent<BoidManager>().SendReformSignal();
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

    public virtual void FinishAppearing(){
        if (!overrideMeshRenderer){
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
}}}