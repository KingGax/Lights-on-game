using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LightableTargetDummy : LightableEnemy
{
    public Transform parentTransform;
    public bool useParentTransform;
    public override void SetColour() {
        
    }
    [PunRPC]
    protected override void InitialiseEnemyRPC(LightableColour newCol, string parentName) {
        if (useParentTransform) {
            parentTransform.SetParent(GameObject.Find(parentName).transform);
        }
        else {
            transform.parent.SetParent(GameObject.Find(parentName).transform);
        }
    }
}
