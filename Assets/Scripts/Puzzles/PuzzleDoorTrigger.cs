using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightsOn.LightingSystem;

public class PuzzleDoorTrigger : MonoBehaviour {

    public bool unlocked = false;
    public LightableExitDoor door;
    public LightColour ballColour;
    public Vector3 ballPos;

    //Checks if the puzzle objective is complete and unlocks the door if so
    void OnTriggerEnter(Collider other) { 
        if (other.gameObject.layer == 17) {
            if (!unlocked) {
                ballColour = other.transform.gameObject.GetComponentInChildren<LightablePuzzleBall>().GetColour();
                ballPos = other.transform.position;
                //door.PuzzleBallUnlockDoor(ballColour,ballPos);
                other.transform.gameObject.GetComponent<BouncyBall>().DestroyBall();
                unlocked = true;
            } else {
                other.transform.gameObject.GetComponent<BouncyBall>().Respawn();
            }
        }
    }
}