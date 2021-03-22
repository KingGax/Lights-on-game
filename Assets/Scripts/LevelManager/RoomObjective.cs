using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomObjective : MonoBehaviour
{
    protected PhotonView pv;
    public List<LightableExitDoor> exits;
    public List<LightableExitDoor> entrances;
    //public GameObject EndTooltip;
    // Start is called before the first frame update
    void Start()
    {
        pv = gameObject.GetPhotonView();
    }

    public virtual void StartObjective() {
        
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
        pv.RPC("LockExitLocal", RpcTarget.All);
    }
    protected void UnlockExitGlobal() {
        pv.RPC("UnlockExitLocal", RpcTarget.All);
    }

    protected void UnlockEntrancesGlobal() {
        pv.RPC("UnlockEntrancesLocal", RpcTarget.All);
    }
    protected void LockEntrancesGlobal() {
        pv.RPC("LockEntrancesLocal", RpcTarget.All);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
