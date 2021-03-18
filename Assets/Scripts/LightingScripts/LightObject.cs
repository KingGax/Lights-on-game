using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Light))]
[RequireComponent(typeof(SphereCollider))]
public class LightObject : MonoBehaviour {

    public Color colour;
    public LightableColour colour1;
    Light playerLantern;
    float lightRange;
    private float range;
    SphereCollider sphere;
    int lightLayer;

    public void Awake() {
        colour1 = LightableColour.Red;
    }

    public void Start() {
        playerLantern = GetComponent<Light>();
        colour = playerLantern.color;
        sphere = GetComponent<SphereCollider>();
        lightRange = playerLantern.range;
        range = lightRange / 1.8f;
        sphere.radius = range;
        lightLayer = 1 << LayerMask.NameToLayer("LightingHitboxes");
    }

    public void Update() {
        /*playerLantern = GetComponent<Light>();
        sphere = GetComponent<SphereCollider>();
        lightRange = playerLantern.range;
        sphere.radius = lightRange / 1.3f;*/
    }

    public float GetRange() {
        return range;
    }

    private void UpdateMyColour() {
        colour = playerLantern.color;
    }

    public void ChangeColour() {
        UpdateMyColour();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position+sphere.center, sphere.radius,lightLayer);
        foreach (var hitCollider in hitColliders) {
            LightableObject ls = hitCollider.GetComponent<LightableObject>();
            if (ls != null) {
                ls.ColourChanged();
            }
        }
    }
}