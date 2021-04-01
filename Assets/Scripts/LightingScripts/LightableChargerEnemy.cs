using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightableChargerEnemy : LightableEnemy
{
    public SkinnedMeshRenderer smr;
    Animator animator;

    public override void Start() {
        overrideMeshRenderer = true;
        animator = transform.parent.GetComponent<Animator>();
        base.Start();
    }
    public override void SetColour()
    {
        base.SetColour();
        if (initialised) {
            smr.material = defaultMaterial;
        }
    }

    public override void Disappear() {
        animator.speed = 0f;
        smr.material = hiddenMaterials.get(colour);
        smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        base.Disappear();
    }
    public override void Appear() {
        animator.speed = 1f;
        smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        smr.material = defaultMaterial;
        base.Appear();
    }
}
