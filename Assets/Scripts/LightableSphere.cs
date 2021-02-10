using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LightableSphere : MonoBehaviour
{
    public bool isRed;
    public bool isBlue;
    public bool isGreen;
    public Material hiddenMaterial;
    Material defaultMaterial;
    public float colourRange;
    float invisibleOpacity = 0.1f;
    public LayerMask potentialColliders;
    bool isHidden = false;
    bool appearing = false;
    Bounds physicsBounds;
    float boundingSphereSize;
    int defaultLayer;
    int hiddenLayer;
    Color objectColour;
    Vector4 objectColVector;
    
    Vector4 materialColour;
    
    List<LightObject> currentLights = new List<LightObject>();
    MeshRenderer meshRenderer;
    Collider physicsCollider;
    private void Awake()
    {
        physicsCollider = transform.parent.GetComponent<Collider>();
    }
    // Start is called before the first frame update
    void Start()
    {
        potentialColliders = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Enemies"));
        defaultLayer = transform.parent.gameObject.layer;
        hiddenLayer = LayerMask.NameToLayer("HiddenObjects");
        meshRenderer = transform.parent.GetComponent<MeshRenderer>();
        physicsBounds = physicsCollider.bounds;
        boundingSphereSize = Mathf.Max(physicsBounds.size.x, physicsBounds.size.y, physicsBounds.size.z);
        materialColour = meshRenderer.material.color;
        defaultMaterial = meshRenderer.material;
        objectColour = CalculateColour(isRed, isGreen, isBlue);
        objectColVector = objectColour;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void StartAppearing()
    {
        if (isHidden && !appearing)
        {
            appearing = true;
            InvokeRepeating("TryAppear", 0f, 0.1f);
        }
    }
    void TryAppear()
    {
        if (CheckNoIntersections())
        {
            Appear();
            CancelInvoke("TryAppear");
            appearing = false;
        }
    }

    public void ColourChanged()
    {
        if (CheckColours(currentLights))
        {
            Disappear();
        }
        else
        {
            StartAppearing();
        }
        
    }

    bool CheckNoIntersections()
    {
        physicsBounds.center = transform.position;
        Collider[] closeColliders = Physics.OverlapSphere(transform.position, boundingSphereSize, potentialColliders);
        foreach (Collider col in closeColliders)
        {
            Debug.Log(col.gameObject.name);
            if (physicsBounds.Intersects(col.bounds))
            {
                return false;
            }
        }
        return true;
    }

    //Returns true if colours match - only deals with one colour currently
    bool CheckColours(List<LightObject> lights) {
        if (lights.Count == 0)
        {
            return false;
        }

        Vector4 lightColour = Vector4.zero;
        for(int i = 0; i < lights.Count; i++) {
            lightColour += (Vector4)lights[i].colour;
        }
        lightColour = new Vector4(Mathf.Clamp(lightColour.x, 0.0f, 1.0f), Mathf.Clamp(lightColour.y, 0.0f, 1.0f), Mathf.Clamp(lightColour.z, 0.0f, 1.0f), 1.0f);

        Vector4 lightColVector = lightColour;
        Vector4 objectColour = lightColour;
        Vector4 colourDif = lightColVector - objectColVector;
        return colourDif.magnitude <= colourRange;
    }

    Color CalculateColour(bool red, bool green, bool blue)
    {
        float r = red ? 1 : 0;
        float g = green ? 1 : 0;
        float b = blue ? 1 : 0;
        return new Color(r, g, b);
    }
    void OnTriggerEnter(Collider other) {
        LightObject newLight = other.GetComponent<LightObject>();
        if (newLight != null)
        {
            currentLights.Add(newLight);
            if (CheckColours(currentLights))
            {
                Disappear();
            }
            else
            {
                StartAppearing();
            }
        }
        
        
    }
    void Disappear()
    {
        if (!isHidden)
        {
            isHidden = true;
            appearing = false;
            CancelInvoke("TryAppear");
            meshRenderer.material = hiddenMaterial;
            transform.parent.gameObject.layer = hiddenLayer;
        }
        if (appearing)
        {
            appearing = false;
            CancelInvoke("TryAppear");
        }
    }
    void Appear()
    {
        isHidden = false;
        transform.parent.gameObject.layer = defaultLayer;
        meshRenderer.material = defaultMaterial;
    }

    void OnTriggerExit(Collider other)
    {
        LightObject newLight = other.GetComponent<LightObject>();
        if (newLight != null)
        {
            currentLights.Remove(newLight);
            if (CheckColours(currentLights))
            {
                Disappear();
            }
            else
            {
                StartAppearing();
            }
        }
    }
}
