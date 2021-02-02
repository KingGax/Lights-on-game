using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LightableSphere : MonoBehaviour
{
    public bool isRed;
    public bool isBlue;
    public bool isGreen;
    Color objectColour;
    Vector4 objectColVector;
    public float colourRange;

    List<LightObject> currentLights = new List<LightObject>();
    MeshRenderer meshRenderer;
    SphereCollider physicsSphereCollider;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = transform.parent.GetComponent<MeshRenderer>();
        physicsSphereCollider = transform.parent.GetComponent<SphereCollider>();
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
        Color lightColour = lights[0].colour;
        Vector4 lightColVector = lightColour;
        Vector4 objectColour = lightColour;
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
        meshRenderer.enabled = false;
        physicsSphereCollider.enabled = false;
    }
    void Appear()
    {
        meshRenderer.enabled = true;
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
