using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRange : MonoBehaviour
{
    Light playerLantern;
    float lightRange;
    SphereCollider sphere;

    // Start is called before the first frame update
    void Start()
    {
        playerLantern = GetComponent<Light>();
        sphere = GetComponent<SphereCollider>();
        lightRange = playerLantern.range;
        sphere.radius = lightRange;
    }

    // Update is called once per frame
    void Update()
    {
        playerLantern = GetComponent<Light>();
        sphere = GetComponent<SphereCollider>();
        lightRange = playerLantern.range;
        sphere.radius = lightRange;
    }
}
