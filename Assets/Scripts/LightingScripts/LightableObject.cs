using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LightableColour {
    Red,
    Green,
    Blue,
    Cyan,
    Magenta,
    Yellow,
}

public class LightableObject : MonoBehaviour {
    public LightableColour colour;
    public Material greenMat;
    public Material blueMat;
    public Material redMat;
    public Material magentaMat;
    public Material cyanMat;
    public Material yellowMat;
    protected Material hiddenMaterial;
    protected Material defaultMaterial;
    protected bool initialised = false;
    protected bool disappearOnStart = false;

    public float colourRange;
    float invisibleOpacity = 0.1f;
    protected LayerMask potentialColliders;
    bool isHidden = false;
    bool appearing = false;
    Bounds physicsBounds;
    float boundingSphereSize;
    int defaultLayer;
    int hiddenLayer;
    Color objectColour;
    Vector4 objectColVector;

    List<LightObject> currentLights = new List<LightObject>();
    MeshRenderer meshRenderer;
    Collider physicsCollider;

    virtual protected void Awake() {
        hiddenLayer = LayerMask.NameToLayer("HiddenObjects");
    }

    void Start() {
        AssignMaterials();
        meshRenderer = transform.parent.GetComponent<MeshRenderer>();
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
                if (physicsBounds.Intersects(lanternCol.bounds)) {
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
            default:
                break;
        }
        objectColour = CalculateColour();
        objectColVector = objectColour;
        if (initialised) {
            meshRenderer.material = defaultMaterial;
        }
    }

    public virtual bool CheckNoIntersections() {
        physicsBounds.center = transform.position;
        Collider[] closeColliders = Physics.OverlapSphere(transform.position, boundingSphereSize, potentialColliders);
        foreach (Collider col in closeColliders) {
            if (physicsBounds.Intersects(col.bounds)) {
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
        for (int i = 0; i < lights.Count; i++) {
            lightColour += (Vector4)lights[i].colour;
        }

        lightColour = new Vector4(Mathf.Clamp(lightColour.x, 0.0f, 1.0f), Mathf.Clamp(lightColour.y, 0.0f, 1.0f), Mathf.Clamp(lightColour.z, 0.0f, 1.0f), 1.0f);

        Vector4 lightColVector = lightColour;
        Vector4 objectColour = lightColour;
        Vector4 colourDif = lightColVector - objectColVector;
        return colourDif.magnitude <= colourRange;
    }

    Color CalculateColour() {
        switch (colour) {
            case LightableColour.Red:
                return new Color(1, 0, 0);
            case LightableColour.Green:
                return new Color(0, 1, 0);
            case LightableColour.Blue:
                return new Color(0, 0, 1);
            case LightableColour.Cyan:
                return new Color(0, 1, 1);
            case LightableColour.Magenta:
                return new Color(1, 0, 1);
            case LightableColour.Yellow:
                return new Color(1, 1, 0);
            default:
                return new Color(1, 0, 0); ;
        }
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
        meshRenderer.material = hiddenMaterial;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        transform.parent.gameObject.layer = hiddenLayer;
    }
    public virtual void Appear() {
        transform.parent.gameObject.layer = defaultLayer;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        meshRenderer.material = defaultMaterial;
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
        }
        else {
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
}