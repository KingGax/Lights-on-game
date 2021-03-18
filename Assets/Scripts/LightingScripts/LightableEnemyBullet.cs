using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightableEnemyBullet : LightableObject
{
    TrailRenderer bulletTrail;
    // Start is called before the first frame update
    public override void Start() {
        base.Start();
        bulletTrail = transform.parent.GetComponentInChildren<TrailRenderer>();
    }

    public override void Appear() {
        base.Appear();
        bulletTrail.emitting = true;
    }

    public override void Disappear() {
        base.Disappear();
        bulletTrail.emitting = false;
    }
}
