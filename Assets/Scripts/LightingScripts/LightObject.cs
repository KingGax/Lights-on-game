using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LightObject : MonoBehaviour {
    // Start is called before the first frame update

    public Color colour { get; set; }
    Light playerLantern;
    float lightRange;
    SphereCollider sphere;
    int lightLayer;

    void Start() {
        playerLantern = GetComponent<Light>();
        colour = playerLantern.color;
        playerLantern = GetComponent<Light>();
        sphere = GetComponent<SphereCollider>();
        lightRange = playerLantern.range;
        sphere.radius = lightRange / 1.3f;
        lightLayer = 1 << LayerMask.NameToLayer("LightingHitboxes");
    }

    // Update is called once per frame
    void Update() {
        playerLantern = GetComponent<Light>();
        sphere = GetComponent<SphereCollider>();
        lightRange = playerLantern.range;
        sphere.radius = lightRange / 1.3f;
    }

    public void ChangeColour() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position+sphere.center, sphere.radius,lightLayer);
        foreach (var hitCollider in hitColliders) {
            LightableObject ls = hitCollider.GetComponent<LightableObject>();
            if (ls != null) {
                ls.ColourChanged();
            }
        }
    }
}