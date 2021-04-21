using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using LightsOn.LightingSystem;

public class BossController : Enemy
{
    public GameObject bullet;
    public float bulletSpeed;
    LightColour bulletColour;

    public float aoeStartTimerMax; 
    float aoeStartTimer;
    public float aoeEndTimerMax;
    float aoeEndTimer;
    public float rotatingShotTimerMax;
    float rotatingShotTimer;
    public float reappearingTimerMax;
    float reappearingTimer;
    public float missileShotsMax;
    float missileShotsFired;
    EnemyState enemyState;
    float pathStoppingThreshold = 0.01f;
    float staggerCount;
    public float staggerCountMax;
    int currentPhase;

    enum EnemyState {
        DecisionState, //Base state - deciding what to do
        RotateShooting, //Firing rotating shots
        MissileAttack, //Using missile attack
        Patrolling, //Moving/idle state - hasn't engaged the player yet 
        AOEMeleeStartup, //Area-of-effect melee attack startup
        AOEMelee, //actively attacking
        AOEMeleeRecovery, //recovery from AOE melee attack
        SummonAdds, //Currently summoning minions
        SwarmRepositioning, //Moving during combat
        Reappearing, //Reapearring and performing AoE attack
        Staggered //Staggered/stunned (after taking a certain amount of damage) [not sure if I want to use this]
    }

    void Start()
    {
        currentPhase = 0; 
        enemyState = EnemyState.Patrolling;
        StartCoroutine("EnemyTimers");
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pv == null || !pv.IsMine) return;
        // if (!hasPlayerJoined){
        //     if (GlobalValues.Instance != null && GlobalValues.Instance.players.Count > 0){
        //         hasPlayerJoined = true;
        //         int index = SelectTarget();
        //         weapon.SetTarget(index);
        //     } else {
        //         return;
        //     }
        // } 
        ManageStates();
    }

    void ManageStates(){
        if (aiEnabled) {
            switch (enemyState) {
                case EnemyState.DecisionState:
                    MakeDecision();
                    break;
                case EnemyState.RotateShooting:
                    RotateShoot();
                    break;
                case EnemyState.MissileAttack:
                    MissileAttack();
                    break;
                case EnemyState.Patrolling:
                    Patrol();
                    break;
                case EnemyState.AOEMeleeStartup:
                    AOEMeleeStartup();
                    break;
                case EnemyState.AOEMelee:
                    AOEMelee();
                    break;
                case EnemyState.AOEMeleeRecovery:
                    AOEMeleeRecovery();
                    break;
                case EnemyState.SummonAdds:
                    SummonAdds();
                    break;
                case EnemyState.SwarmRepositioning:
                    SwarmReposition();
                    break;
                case EnemyState.Reappearing:
                    ReappearState();
                    break;
                default:
                    break;
            }
        }
    }

    void MakeDecision(){

    }

    void ChangeToRotateShoot(){
        enemyState = EnemyState.RotateShooting;
    }

    void RotateShoot(){

    }

    void ChangeToMissileAttack(){
        enemyState = EnemyState.MissileAttack;
        missileShotsFired = 0;
    }

    void MissileAttack(){

    }

    void Patrol(){

    }

    void ChangeToAOEMeleeStartup(){
        enemyState = EnemyState.AOEMeleeStartup;
        aoeStartTimer = aoeStartTimerMax;
    }

    void AOEMeleeStartup(){

    }

    void ChangeToAOEMelee(){
        enemyState = EnemyState.AOEMelee;
    }

    void AOEMelee(){

    }
    void ChangeToAOEMeleeRecovery(){
        enemyState = EnemyState.AOEMeleeRecovery;
        aoeEndTimer = aoeEndTimerMax;
    }

    void AOEMeleeRecovery(){

    }

    void ChangeToSummonAdds(){
        enemyState = EnemyState.SummonAdds;
    }

    void SummonAdds(){

    }

    void ChangeToSwarmReposition(){
        enemyState = EnemyState.SwarmRepositioning;
    }
    
    void SwarmReposition(){

    }

    void ChangeToReappearState(){
        enemyState=EnemyState.Reappearing;
    }

    void ReappearState(){

    }

    // Start is called before the first frame update
    

    private IEnumerator EnemyTimers() {
        while (true) {
            if (aoeStartTimer > 0) {
                aoeStartTimer -= Time.deltaTime;
            }

            if (aoeEndTimer > 0) {
                aoeEndTimer -= Time.deltaTime;
            }
            if (rotatingShotTimer > 0) {
                rotatingShotTimer -= Time.deltaTime;
            }
            if (reappearingTimer > 0) {
                reappearingTimer -= Time.deltaTime;
            }
            yield return null;
        }
    }
}
