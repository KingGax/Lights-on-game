using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(NavMeshAgent))]
public abstract class Enemy : MonoBehaviour {
    
    protected PhotonView pv;
    protected NavMeshAgent agent;
    protected bool aiEnabled;
    public Weapon weapon;
    public float turnSpeed;
    protected GameObject playerObj;
    protected bool hasPlayerJoined;
    private LayerMask environmentAndPlayerMask;

    public virtual void Awake() {
        Debug.Log("okkokokoko");
        pv = GetComponent<PhotonView>();
        weapon = GetComponentInChildren<Weapon>();
        agent = GetComponent<NavMeshAgent>();
        environmentAndPlayerMask = (1 << LayerMask.NameToLayer("Player"))
            | (1 << LayerMask.NameToLayer("StaticEnvironment"))
            | (1 << LayerMask.NameToLayer("DynamicEnvironment"));
        EnableAI();
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

    protected int SelectTarget(){ //default implementation sets target as closest player
        float minDist = Mathf.Infinity;
        int targetIndex = 0;
        for(int i = 0; i < GlobalValues.Instance.players.Count; i++){
            float distToPlayer = Vector3.Distance(gameObject.transform.position, GlobalValues.Instance.players[i].transform.position);
            if (distToPlayer < minDist){
                Debug.Log("Player distance: "+ distToPlayer);
                minDist = distToPlayer;
                targetIndex = i;
            }
        }
        Debug.Log("Player index: "+targetIndex);
        playerObj = GlobalValues.Instance.players[targetIndex];
        return targetIndex;
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

    protected void TurnTowards(Vector3 direction)
    {
        transform.forward = Vector3.RotateTowards(transform.forward, direction, Time.deltaTime * turnSpeed, 0.0f);
    }
}
