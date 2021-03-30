using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightableObject : MonoBehaviour {

    public LightableColour colour;

    public Material greenMat;
    public Material blueMat;
    public Material redMat;
    public Material magentaMat;
    public Material cyanMat;
    public Material yellowMat;
    public Material whiteMat;
    protected Material hiddenMaterial;
    protected Material defaultMaterial;

    protected bool initialised = false;
    protected bool disappearOnStart = false;
    protected bool overrideMeshRenderer = false;

    public float colourRange;
    float invisibleOpacity = 0.1f;
    protected LayerMask potentialColliders;
    bool isHidden = false;
    bool appearing = false;
    Bounds physicsBounds;
    float boundingSphereSize;
    protected int defaultLayer;
    int hiddenLayer;
    Color objectColour;
    Vector4 objectColVector;

    bool distCheckThisFrame = false;
    float lightAlwaysConsideredDist = 2f;
    float lightOverpowerRatio = 1.4f;

    List<LightObject> currentLights = new List<LightObject>();
    MeshRenderer meshRenderer;
    Collider physicsCollider;
    GameObject boidManagerPrefab;
    GameObject boidManagerInstance;
    protected bool canSwarm;

    virtual protected void Awake() {
        hiddenLayer = LayerMask.NameToLayer("HiddenObjects");
    }

    public virtual void Start() {
        canSwarm = true;
        boidManagerPrefab = GlobalValues.Instance.boidManagerPrefab;
        AssignMaterials();
        if (!overrideMeshRenderer) {
            meshRenderer = transform.parent.GetComponent<MeshRenderer>();
        }
        physicsCollider = transform.parent.GetComponent<Collider>();
        potentialColliders = GlobalValues.Instance.reappearPreventionLayers;
        defaultLayer = transform.parent.gameObject.layer;
        physicsBounds = physicsCollider.bounds;
        boundingSphereSize = Mathf.Max(physicsBounds.size.x, physicsBounds.size.y, physicsBounds.size.z);
        objectColour = CalculateColour();
        objectColVector = objectColour;
        hiddenMaterial = GetHiddenMaterial();
        initialised = true;
        SetColour();
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
        distCheckThisFrame = !distCheckThisFrame;
    }

    void AssignMaterials() {
        GlobalValues gv = GlobalValues.Instance;
        if (greenMat == null) {
            greenMat = gv.defaultGreen;
        }

        if (redMat == null) {
            redMat = gv.defaultRed;
        }

        if (blueMat == null) {
            blueMat = gv.defaultBlue;
        }
    }

    Material GetHiddenMaterial() {
        GlobalValues gv = GlobalValues.Instance;
        switch (colour) {
            case LightableColour.Red:
                return gv.hiddenRed;
            case LightableColour.Green:
                return gv.hiddenGreen;
            case LightableColour.Blue:
                return gv.hiddenBlue;
            case LightableColour.Cyan:
                return gv.hiddenCyan;
            case LightableColour.Magenta:
                return gv.hiddenMagenta;
            case LightableColour.Yellow:
                return gv.hiddenYellow;
            default:
                return gv.hiddenRed;
        }
    }

    void GetLightsInRange() {
        for (int i = 0; i < GlobalValues.Instance.players.Count; i++) {
            LightObject currentLantern = GlobalValues.Instance.players[i].GetComponentInChildren<LightObject>();
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
        Vector4 lightColour = new Vector4(0, 0, 0, 1f);
        foreach (LightObject lo in currentLights) {
            float dist = Vector3.Distance(transform.position, lo.gameObject.transform.position);
            if (closestLight > dist) {
                closestLight = dist;
            }
        }

        foreach (LightObject lo in currentLights) {
            float dist = Vector3.Distance(transform.position, lo.gameObject.transform.position);
            if (dist < lightAlwaysConsideredDist || dist < lightOverpowerRatio * closestLight) {
                lightColour += (Vector4)lo.colour;
            }
        }

        lightColour = new Vector4(Mathf.Clamp(lightColour.x, 0.0f, 1.0f), Mathf.Clamp(lightColour.y, 0.0f, 1.0f), Mathf.Clamp(lightColour.z, 0.0f, 1.0f), 1.0f);
        Vector4 lightColVector = lightColour;
        Vector4 colourDif = lightColVector - objectColVector;
        return colourDif.magnitude <= colourRange;
        
    }

    public void ColourChanged() {
        if (CheckColours(currentLights)) {
            StartDisappear();
        } else {
            StartAppearing();
        }
    }

    public Material GetDefaultMaterial() {
        return defaultMaterial;
    }

    public virtual void SetColour() {
        switch (colour) {
            case LightableColour.Red:
                defaultMaterial = redMat;
                break;
            case LightableColour.Green:
                defaultMaterial = greenMat;
                break;
            case LightableColour.Blue:
                defaultMaterial = blueMat;
                break;
            case LightableColour.Cyan:
                defaultMaterial = cyanMat;
                break;
            case LightableColour.Magenta:
                defaultMaterial = magentaMat;
                break;
            case LightableColour.Yellow:
                defaultMaterial = yellowMat;
                break;
            case LightableColour.White:
                defaultMaterial = whiteMat;
                break;
            default:
                break;
        }

        objectColour = CalculateColour();
        objectColVector = objectColour;
        if (initialised && !overrideMeshRenderer) {
            meshRenderer.material = defaultMaterial;
            GetHiddenMaterial();
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
    bool CheckColours(List<LightObject> lights) {
        if (lights.Count == 0) {
            return false;
        }

        Vector4 lightColour = Vector4.zero;
        for (int i = 1; i < lights.Count; i++) {
            lightColour += (Vector4)lights[i].colour;
        }

        lightColour = new Vector4(Mathf.Clamp(lightColour.x, 0.0f, 1.0f), Mathf.Clamp(lightColour.y, 0.0f, 1.0f), Mathf.Clamp(lightColour.z, 0.0f, 1.0f), 1.0f);

        Vector4 lightColVector = lightColour;
        Vector4 colourDif = lightColVector - objectColVector;
        return colourDif.magnitude <= colourRange;
    }

    Color CalculateColour() {
        return colour.ToColor();
    }

    void OnTriggerEnter(Collider other) {
        LightObject newLight = other.GetComponent<LightObject>();
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

    public virtual void Disappear() {
        if (!overrideMeshRenderer) {
            meshRenderer.material = hiddenMaterial;
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        transform.parent.gameObject.layer = hiddenLayer;
        if (canSwarm){
            boidManagerInstance = Instantiate(boidManagerPrefab, transform.position, transform.rotation);
            BoidManager man = boidManagerInstance.GetComponent<BoidManager>();
            man.SetMat(CalculateColour());
            man.Spawn();
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
            meshRenderer.material = defaultMaterial;
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
        transform.parent.gameObject.layer = defaultLayer;
        if (canSwarm){
            Destroy(boidManagerInstance);
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

    void OnTriggerExit(Collider other) {
        LightObject newLight = other.GetComponent<LightObject>();
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