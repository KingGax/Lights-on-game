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

    Color objectColour;
    Vector4 objectColVector;
    
    Vector4 materialColour;
    
    List<LightObject> currentLights = new List<LightObject>();
    MeshRenderer meshRenderer;
    SphereCollider physicsSphereCollider;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = transform.parent.GetComponent<MeshRenderer>();
        physicsSphereCollider = transform.parent.GetComponent<SphereCollider>();
        materialColour = meshRenderer.material.color;
        defaultMaterial = meshRenderer.material;
        objectColour = CalculateColour(isRed, isGreen, isBlue);
        objectColVector = objectColour;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ColourChanged()
    {
        if (CheckColours(currentLights))
        {
            Disappear();
        }
        else
        {
            Appear();
        }
        
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

        /*float hue;
        float sat;
        float value;
        Color.RGBToHSV(lightColour, out hue, out sat, out value);
        if(sat > 0.5 && value > 0.8 && (hue < 0.1 || hue > 0.9)) {
            //disappear
            return true;
            
        } else {
            return false;
            
        }*/
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
                Appear();
            }
        }
        
        
    }
    void Disappear()
    {
        //meshRenderer.material.color = new Vector4(materialColour.x, materialColour.y,materialColour.z, invisibleOpacity);
        meshRenderer.material = hiddenMaterial;
        physicsSphereCollider.enabled = false;
    }
    void Appear()
    {

        //meshRenderer.material.color = new Vector4(materialColour.x, materialColour.y,materialColour.z, 1f);
        meshRenderer.material = defaultMaterial;
        physicsSphereCollider.enabled = true;
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
                Appear();
            }
        }
    }


}
