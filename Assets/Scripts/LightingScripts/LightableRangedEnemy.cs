using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightableRangedEnemy : LightableEnemy
{
    public SkinnedMeshRenderer smr;
    public EnemyGun gunScript;
    private Animator anim;
    public bool usesMeshRenderer = false; //allows for non-modelled ranged enemies, remove once sniper has a model

    public override void Start() {
        overrideMeshRenderer = !usesMeshRenderer;
        anim = transform.parent.GetComponent<Animator>();
        base.Start();
    }
    public override void SetColour()
    {
        base.SetColour();
        gunScript.SetColour(colour);
        if (initialised) {
            smr.material = defaultMaterial;
        }
    }

    public override void Disappear() {
        anim.speed = 0;
        smr.material = hiddenMaterials.get(colour);
        smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        base.Disappear();
    }
    public override void Appear() {
        anim.speed = 1;
        smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        smr.material = defaultMaterial;
        base.Appear();
    }
}
