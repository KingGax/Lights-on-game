using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using LightsOn.WeaponSystem;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(NavMeshAgent))]
public abstract class Enemy : MonoBehaviourPunCallbacks { //enemy AI base class
    
    protected PhotonView pv;
    protected NavMeshAgent agent;
    protected Animator animator;
    protected bool aiEnabled;
    public Weapon weapon;
    public float turnSpeed;
    public bool ignoresStun;
    protected bool inStunnableState;
    protected GameObject playerObj;
    protected bool hasPlayerJoined;
    private LayerMask environmentAndPlayerMask;

    public virtual void Awake() { //Get components, setup mask and enable AI
        pv = GetComponent<PhotonView>();
        weapon = GetComponentInChildren<Weapon>();
        agent = GetComponent<NavMeshAgent>();
        environmentAndPlayerMask = (1 << LayerMask.NameToLayer("Player"))
            | (1 << LayerMask.NameToLayer("StaticEnvironment"))
            | (1 << LayerMask.NameToLayer("DynamicEnvironment"));
        EnableAI();
    }

    

    public virtual void EnableAI() { //enable AI processing
        if (pv.IsMine) {
            agent.enabled = true;
            agent.isStopped = false;
            aiEnabled = true;
        }
    }

    public virtual void DisableAI() { //disable AI processing
        if (pv.IsMine) {
            aiEnabled = false;
            agent.enabled = false;
        }
    }

    public virtual void RequestHitStun(float duration){ //default hitstun handler
        if (inStunnableState){
            agent.velocity = new Vector3(0,agent.velocity.y,0);
            if (agent.enabled){
                agent.isStopped = true;
            }
            Invoke("Unstop", duration);
        }
    }

    void Unstop(){
        if (agent.enabled){
            agent.isStopped = false;
        }
    }

    protected int SelectTarget(){ //default implementation sets target as closest player
        float minDist = Mathf.Infinity;
        int targetIndex = 0;
        for(int i = 0; i < GlobalValues.Instance.players.Count; i++){
            float distToPlayer = Vector3.Distance(gameObject.transform.position, GlobalValues.Instance.players[i].transform.position);
            if (distToPlayer < minDist){
                minDist = distToPlayer;
                targetIndex = i;
            }
        }
        playerObj = GlobalValues.Instance.players[targetIndex];
        return targetIndex;
    }

    protected bool HasPlayerLOS(GameObject playerObj, float sightRange) { //check if player is in line of sight of enemy
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
    

    protected void TurnTowards(Vector3 direction) { //turn towards direction
        transform.forward = Vector3.RotateTowards(transform.forward, direction, Time.deltaTime * turnSpeed, 0.5f);
    }

}
