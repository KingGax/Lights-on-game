using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightableSphere : MonoBehaviour
{

    
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshCollider = gameObject.GetComponent<MeshCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckColours(List<LightObject> lights) {
        Color lightColour = lights[0].colour;
        float hue;
        float sat;
        float value;
        Color.RGBToHSV(lightColour, out hue, out sat, out value);
        if(value > 0.8 && hue ) {
            //disappear
            meshRenderer.enabled = false;
            meshCollider.enabled = false;
        } else {
            meshRenderer.enabled = true;
            meshCollider.enabled = true;
        }
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log(other.GetComponent<LightObject>().colour);

        CheckColours(new List<LightObject>(){other.GetComponent<LightObject>()});
    }
}
