using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightableMultiObject : LightableObject
{
    public List<Renderer> childObjects = null;
    // Start is called before the first frame update
    public override void SetColour()
    {
        base.SetColour();
        if (initialised) {
            if (childObjects != null) {
                // foreach (MeshRenderer mr in childObjects) {
                //     mr.material = GetDefaultMaterial();
                // }
                foreach (Renderer r in childObjects) {
                    r.material = GetDefaultMaterial();
                }
            }
        }
    }

    public override void Disappear()
    {
        base.Disappear();
        if (childObjects != null)
        {
            foreach (Renderer mr in childObjects)
            {
                mr.material = hiddenMaterial;
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }
        
    }
    public override void Appear()
    {
        base.Appear();
        if (childObjects != null)
        {
            foreach (Renderer mr in childObjects)
            {
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                mr.material = defaultMaterial;
            }
        }
    }
}
