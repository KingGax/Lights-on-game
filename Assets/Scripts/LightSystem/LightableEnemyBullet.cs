using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LightsOn {
namespace LightingSystem {

public class LightableEnemyBullet : LightableObject {

    public TrailRenderer bulletTrail;
    public override void Start() {
        base.Start();
        canSwarm = false;
    }

    public override void Appear() {
        base.Appear();
        if (bulletTrail != null) bulletTrail.emitting = true;
        
    }

    public override void Disappear() {
        base.Disappear();
         if (bulletTrail != null) bulletTrail.emitting = false;
    }
}}}