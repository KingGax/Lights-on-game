using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TargetDummyController : Enemy {

    EnemyState enemyState;

    enum EnemyState {
        Patrolling, //Only target state
    }

    // Start is called before the first frame update
    void Start() {
        agent = GetComponent<NavMeshAgent>();
        enemyState = EnemyState.Patrolling;
        inStunnableState = true;
    }

    public override void Awake() {
        base.Awake();
        if (GlobalValues.Instance != null && GlobalValues.Instance.players.Count > 0) {
            hasPlayerJoined = true;
        }
    }
    // Update is called once per frame
    /*Always patrol*/
    void Update() {
        if (pv == null || !pv.IsMine) return;
        if (!hasPlayerJoined) {
            if (GlobalValues.Instance != null && GlobalValues.Instance.players.Count > 0) {
                hasPlayerJoined = true;
            }
            else {
                return;
            }
        }
        if (aiEnabled) {
            switch (enemyState) {
                case EnemyState.Patrolling:
                    Patrol();
                    break;
                default:
                    break;
            }
        }
    }

    void Patrol() { //idle state handler (look at player)
        TurnTowards(GlobalValues.Instance.localPlayerInstance.transform.position-transform.position);
    }

    public override void RequestHitStun(float duration) { //doesn't take hitstun

    }

}
