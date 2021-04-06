using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightsOn.LightingSystem;

public class PuzzleDoorTrigger : MonoBehaviour {

    public bool unlocked = false;
    public LightableExitDoor door;

    void OnTriggerEnter(Collider other) { 
        if (other.gameObject.layer == 17) {
            if (!unlocked) {
                door.UnlockDoor();
                other.transform.gameObject.GetComponent<BouncyBall>().DestroyBall();
                unlocked = true;
            } else {
                other.transform.gameObject.GetComponent<BouncyBall>().Respawn();
            }
        }
    }
}