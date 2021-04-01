using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightableMultiObject : LightableObject {
    public List<Renderer> childObjects = null;

    public override void SetColour() {
        base.SetColour();
        if (initialised) {
            if (childObjects != null) {
                foreach (Renderer r in childObjects) {
                    r.material = materials.get(colour);
                }
            }
        }
    }

    public override void Disappear() {
        base.Disappear();
        if (childObjects != null) {
            foreach (Renderer mr in childObjects) {
                mr.material = hiddenMaterials.get(colour);
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }
    }

    public override void Appear() {
        base.Appear();
        if (childObjects != null) {
            foreach (Renderer mr in childObjects) {
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                mr.material = materials.get(colour);
            }
        }
    }
}
