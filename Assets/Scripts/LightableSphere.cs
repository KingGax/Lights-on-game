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
    private LayerMask potentialColliders;
    bool isHidden = false;
    bool appearing = false;

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
        meshRenderer = transform.parent.GetComponent<MeshRenderer>();
        
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
        Collider[] closeColliders = Physics.OverlapSphere(physicsCollider.bounds.center, Mathf.Max(physicsCollider.bounds.size.x,physicsCollider.bounds.size.y,physicsCollider.bounds.size.z),potentialColliders);
        foreach (Collider col in closeColliders)
        {
            if (physicsCollider.bounds.Intersects(col.bounds))
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
        Debug.Log(lightColour);
        Debug.Log(objectColour);
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
        isHidden = true;
        appearing = false;
        CancelInvoke("TryAppear");
        meshRenderer.material = hiddenMaterial;
        physicsCollider.enabled = false;
    }
    void Appear()
    {
        isHidden = false;
        //meshRenderer.material.color = new Vector4(materialColour.x, materialColour.y,materialColour.z, 1f);
        meshRenderer.material = defaultMaterial;
        physicsCollider.enabled = true;
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
