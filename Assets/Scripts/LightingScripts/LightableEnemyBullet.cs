using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightableEnemyBullet : LightableObject
{
    public TrailRenderer bulletTrail;
    public override void Appear() {
        base.Appear();
        bulletTrail.emitting = true;
    }

    public override void Disappear() {
        base.Disappear();
        bulletTrail.emitting = false;
    }
}