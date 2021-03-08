using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Light))]
[RequireComponent(typeof(SphereCollider))]
public class LightObject : MonoBehaviour {
    // Start is called before the first frame update

    public Color colour;
    public LightableColour colour1;
    Light playerLantern;
    float lightRange;
    SphereCollider sphere;
    int lightLayer;

    public void Awake() {
        colour1 = LightableColour.Red;
    }

    void Start() {
        playerLantern = GetComponent<Light>();
        colour = playerLantern.color;
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