using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using LightsOn.LightingSystem;
using LightsOn.AudioSystem;

public class RoomObjective : MonoBehaviour {
    protected PhotonView pv;
    public List<LightableExitDoor> exits;
    public List<LightableExitDoor> entrances;
    public bool complete = false;
    //public GameObject EndTooltip;
    public bool playNextTrack = true;

    void Start() {
        pv = gameObject.GetPhotonView();
    }

    public virtual void StartObjective() {
        
    }

    [PunRPC]
    protected void SetCompleteTrue(){
        complete = true;
        if (playNextTrack)
            AudioManager.Instance.PlayNext();
    }

    [PunRPC]
    public void LockExitLocal() {
        foreach (LightableExitDoor door in exits) {
            door.LockDoor();
        }
    }


    [PunRPC]
    public void UnlockExitLocal() {
        foreach (LightableExitDoor door in exits) {
            door.UnlockDoor();
        }
        //EndTooltip.SetActive(false);
    }

    [PunRPC]

    public void LockEntrancesLocal() {
        foreach (LightableExitDoor door in entrances) {
            door.LockDoor();
        }
    }

    [PunRPC]
    public void UnlockEntrancesLocal() {
        foreach (LightableExitDoor door in entrances) {
            door.UnlockDoor();
        }
    }

    public virtual bool ObjectiveComplete() {
        return true;
    }
    protected void LockExitGlobal() {
        pv.RPC("LockExitLocal", RpcTarget.AllBuffered);
    }
    protected void UnlockExitGlobal() {
        pv.RPC("UnlockExitLocal", RpcTarget.AllBuffered);
    }

    protected void UnlockEntrancesGlobal() {
        pv.RPC("UnlockEntrancesLocal", RpcTarget.AllBuffered);
    }
    protected void LockEntrancesGlobal() {
        pv.RPC("LockEntrancesLocal", RpcTarget.AllBuffered);
    }
}
