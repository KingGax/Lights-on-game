using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PuzzleRoom : RoomObjective
{

    bool started = false;

    bool objectiveComplete = false;

    public PuzzleDoorTrigger trigger;
    // Start is called before the first frame update
    void Start()
    {
        pv = gameObject.GetPhotonView();
    }

    public override void StartObjective() {
        Debug.Log("started objective");
        started = true;
        LockEntrancesGlobal();
        LockExitGlobal();
        // StartNewSetWave();
    }
    // Update is called once per frame
    void Update()
    {
        if (pv == null || !pv.IsMine) return;
        if(!objectiveComplete && trigger.unlocked){
                UnlockExitGlobal();
                objectiveComplete = true;
        }
    }
}
