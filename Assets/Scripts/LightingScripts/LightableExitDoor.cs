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
        disappeared = false;
        transform.parent.gameObject.layer = defaultLayer;
        SetColour(LightableColour.White);
    }

    public override void SetColour(LightableColour col) {
        base.SetColour(col);
        light.color = colour.DoorLightColour();
    }

    public void UnlockDoor() {
        colour = unlockedColour;
        SetColour(unlockedColour);
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
