using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class IGun : MonoBehaviour
{
    public float fireCooldown;
    public virtual void Shoot(Vector3 direction)
    {

    }
}