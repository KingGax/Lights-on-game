using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LightsOn {
namespace LightingSystem {

public class LightableChargerEnemy : LightableEnemy {

    public SkinnedMeshRenderer smr;
    Animator animator;
    ChargerEnemyController controller;

    public override void Start() {
        overrideMeshRenderer = true;
        animator = transform.parent.GetComponent<Animator>();
        controller = transform.parent.GetComponent<ChargerEnemyController>();
        base.Start();
    }

    public override void SetColour(LightColour col) {
        base.SetColour(col);
        if (initialised) {
            smr.material = materials.get(colour);
        }
    }

    public override void Disappear() {
        controller.Disappear();
        animator.speed = 0f;
        smr.material = hiddenMaterials.get(colour);
        smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        base.Disappear();
    }

    public override void Appear() {
        animator.speed = 1f;
        smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        smr.material = materials.get(colour);
        base.Appear();
        controller.Appear();
    }
}}}