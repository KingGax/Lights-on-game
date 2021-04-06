using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using LightsOn.AudioSystem;

namespace LightsOn {
namespace LightingSystem {

public class LightableExitDoor : LightableObject {
    public Light light;
    public LightColour unlockedColour;
    bool disappeared = false;

    public override void Start() {
        base.Start();
        canSwarm = false;
    }
    public void LockDoor() {
        disappeared = false;
        transform.parent.gameObject.layer = defaultLayer;
        SetColour(LightColour.White);
    }

    public override void SetColour(LightColour col) {
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
}}}