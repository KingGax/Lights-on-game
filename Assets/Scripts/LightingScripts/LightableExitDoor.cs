using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LightableExitDoor : LightableObject
{
    public GameObject lockedLight;
    public GameObject unlockedLight;
    public LightableColour unlockedColour;
    bool disappeared = false;

    // Start is called before the first frame update
    public void LockDoor() {
        colour = LightableColour.White;
        disappeared = false;
        transform.parent.gameObject.layer = defaultLayer;
        SetColour();
        lockedLight.SetActive(true);
        unlockedLight.SetActive(false);

    }
    public void UnlockDoor() {
        colour = unlockedColour;
        SetColour();
        lockedLight.SetActive(false);
        unlockedLight.SetActive(true);
        
    }
    public override void Appear() {
        if (!disappeared) {
            base.Appear();
        }
    }

    public override void Disappear() {
        if (!disappeared) {
            base.Disappear();
            disappeared = true;
        }
    }
}
