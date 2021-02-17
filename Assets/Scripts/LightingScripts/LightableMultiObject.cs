using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightableMultiObject : LightableObject
{
    public List<MeshRenderer> childObjects;
    // Start is called before the first frame update
    public override void SetColour()
    {
        base.SetColour();
        if (childObjects != null)
        {
            foreach (MeshRenderer mr in childObjects)
            {
                mr.material = GetDefaultMaterial();
            }
        }
    }
}
