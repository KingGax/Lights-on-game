using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LightableExitDoor : LightableObject {
    public Light light;
    public LightableColour unlockedColour;
    bool disappeared = false;

    public override void Start() {
        base.Start();
        canSwarm = false;
    }
    public void LockDoor() {
        colour = LightableColour.White;
        disappeared = false;
        transform.parent.gameObject.layer = defaultLayer;
        SetColour();
    }

    public override void SetColour() {
        base.SetColour();
        light.color = colour.DoorLightColour();
    }

    public void UnlockDoor() {
        colour = unlockedColour;
        SetColour();
        AudioManager.PlaySFX(SoundClips.Instance.SFXDoorOpen, transform.position);
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
