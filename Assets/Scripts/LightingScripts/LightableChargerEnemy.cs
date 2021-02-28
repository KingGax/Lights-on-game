using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightableChargerEnemy : LightableEnemy
{
    public SkinnedMeshRenderer smr;

    public override void Start() {
        overrideMeshRenderer = true;
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
        smr.material = hiddenMaterial;
        smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        base.Disappear();
    }
    public override void Appear() {
        smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        smr.material = defaultMaterial;
        base.Appear();
    }
}
