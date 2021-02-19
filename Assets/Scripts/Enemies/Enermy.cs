using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(NavMeshAgent))]
public abstract class Enemy : MonoBehaviour {
    
    protected PhotonView pv;
    protected Weapon weapon;
    protected NavMeshAgent agent;
    protected bool aiEnabled = false;
    private LayerMask environmentAndPlayerMask;

    public void Awake() {
        pv = GetComponent<PhotonView>();
        weapon = GetComponentInChildren<Weapon>();
        agent = GetComponent<NavMeshAgent>();
        environmentAndPlayerMask = (1 << LayerMask.NameToLayer("Player"))
            | (1 << LayerMask.NameToLayer("StaticEnvironment"))
            | (1 << LayerMask.NameToLayer("DynamicEnvironment"));
    }

    public void EnableAI() {
        if (pv.IsMine) {
            agent.enabled = true;
            aiEnabled = true;
        }
    }

    public void DisableAI() {
        if (pv.IsMine) {
            aiEnabled = false;
            agent.enabled = false;
        }
    }

    //TODO make enemies check for LOS in their FOV 
    protected bool HasPlayerLOS(GameObject playerObj, float sightRange) { 
        RaycastHit hit;
        bool environmentCheck = Physics.Raycast(
            transform.position,
            playerObj.transform.position - transform.position,
            out hit,
            sightRange,
            environmentAndPlayerMask
        );

        return !environmentCheck
            || hit.transform.gameObject.layer == LayerMask.NameToLayer("Player");
    }
}
