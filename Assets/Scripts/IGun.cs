using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class IGun : MonoBehaviour {

    [HideInInspector]
    public float fireCooldown;
    public float fireCooldownMax;

    public virtual void Shoot(Vector3 direction) {

    }

    public virtual bool RequestShoot(Vector3 direction) {
        return true;
    }
}