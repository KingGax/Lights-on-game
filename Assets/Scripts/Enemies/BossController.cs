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
    // Start is called before the first frame update
    void Start()
    {
        currentPhase = 0; 
        enemyState = EnemyState.Patrolling;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
