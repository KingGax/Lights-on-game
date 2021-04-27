using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using LightsOn.LightingSystem;
namespace LightsOn.WeaponSystem {
    public class BossController : Enemy {
        [Header("Bullet/weapon setup")]
        public GameObject bullet;
        public float bulletSpeed;
        public int bulletDamage;
        public LightColour bulletColour;
        public List<GameObject> targetGOs;
        public GameObject gunParent;
        public float gunCircleRadius;
        public float bulletTTL;
        public Transform fireOrigin;
        [Header("Attack probabilities")]
        public float rotatingShotProbability; //Probability of starting rotating fire
        public float repositioningProbability; //Probability of repositioning
        public float aoeProbability; //Probability of choosing AoE attack
        public float missileProbability; //Probability of choosing missile attack
        public float summonProbability; //Probability of summoning minions
        [Header("State timers")]
        public float aoeStartTimerMax;
        float aoeStartTimer;
        public float aoeEndTimerMax;
        float aoeEndTimer;
        public float rotatingShotTimerMin;
        public float rotatingShotTimerMax;
        float rotatingShotTimer;
        public float gunCooldownTimerMax;
        float gunCooldownTimer;
        public float changeBulletColTimerMax;
        float changeBulletColTimer;
        public float reappearingTimerMax;
        float reappearingTimer;
        public float summonTimerMax;
        float summonTimer;
        public float movementCooldownTimerMax;
        float movementCooldownTimer;
        EnemyState enemyState;
        EnemyState prevState;
        [Header("Navigation/movement")]
        public float pathStoppingThreshold = 0.5f;
        public float walkRadius;
        public float normalSpeed;
        public float repositionSpeed;

        float staggerCount;
        public float staggerCountMax;
        public float rotationSpeed;
        public float currentGunAngle;
        [Header("AOE attack setup")]
        public float aoeRadius;
        public float aoeDamage;
        public float reappearDamage;
        LineRenderer circleLR;
        [Header("Missile attack setup")]
        public int missileDamage;
        public float missileShotsMax;
        float missileShotsFired;
        public float missileTimeBetweenShots;
        public float missileSpeed;
        public float missileTurnSpeed;
        float missileTimer = 0;
        int currentPhase;
        float cmRepProb;
        float cmAOEProb;
        float cmMissileProb;
        float totalProb;
        int flashNum = 10;
        int flashesRemaining = 0;
        float flashTimerMax;
        float flashTimer;
        bool moving = false;
        List<EnemyGun> rotatingGuns;


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

        void Start() //0.7s, 0.59rad/s
        {
            bulletColour = LightColour.Red;
            currentPhase = 0;
            enemyState = EnemyState.DecisionState;
            StartCoroutine("EnemyTimers");
            agent = GetComponent<NavMeshAgent>();
            //playerObj = GlobalValues.Instance.localPlayerInstance;
            cmRepProb = rotatingShotProbability + repositioningProbability;
            cmAOEProb = cmRepProb + aoeProbability;
            cmMissileProb = cmAOEProb + missileProbability;
            totalProb = cmMissileProb + summonProbability;
            rotatingGuns = new List<EnemyGun>(gunParent.GetComponentsInChildren<EnemyGun>());
            agent.speed = normalSpeed;
            foreach (EnemyGun g in rotatingGuns) {
                g.bulletSpeed = bulletSpeed;
                g.bullet = bullet;
                g.damage = bulletDamage;
                g.bulletTTL = bulletTTL;

            }

            circleLR = GetComponent<LineRenderer>();
            agent.enabled = true;
            moving = false;
        }

        // Update is called once per frame
        void Update() {

            if (pv == null || !pv.IsMine) return;
            if (enemyState != EnemyState.SwarmRepositioning && enemyState != EnemyState.Reappearing) {
                if (moving && agent.remainingDistance < pathStoppingThreshold) {
                    moving = false;
                    movementCooldownTimer = movementCooldownTimerMax;
                }
                if (!moving && movementCooldownTimer <= 0) {
                    Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
                    randomDirection += transform.position;
                    NavMeshHit hit;
                    NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
                    Vector3 finalPosition = hit.position;
                    agent.destination = finalPosition;
                    moving = true;
                }
            }
            if (flashesRemaining > 0 && flashTimer <= 0) {

                if (flashesRemaining % 2 == 0) {
                    circleLR.enabled = false;
                } else {
                    circleLR.enabled = true;
                }
                flashesRemaining--;
                flashTimer = flashTimerMax;
            }
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

        void ManageStates() {
            if (aiEnabled) {
                switch (enemyState) {
                    case EnemyState.DecisionState:
                        float pTotal = rotatingShotProbability + repositioningProbability + aoeProbability + missileProbability + summonProbability;
                        float p = Random.Range(0, pTotal);
                        MakeDecision(p);
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

        void MakeDecision(float p) { //decides which attack/ability to use next, will not use same ability twice in a row
            ChangeToMissileAttack();
            return;


            if (p < rotatingShotProbability) {
                if (prevState == EnemyState.RotateShooting) {
                    float pTotal = rotatingShotProbability + repositioningProbability + aoeProbability + missileProbability + summonProbability;
                    float q = Random.Range(rotatingShotProbability, pTotal + 0);
                    MakeDecision(q % totalProb);
                } else {
                    ChangeToRotateShoot();
                }
            } else if (p < cmRepProb) {
                if (prevState == EnemyState.SwarmRepositioning) {
                    float pTotal = rotatingShotProbability + repositioningProbability + aoeProbability + missileProbability + summonProbability;
                    float q = Random.Range(cmRepProb, pTotal + rotatingShotProbability);
                    MakeDecision(q % totalProb);
                } else {
                    ChangeToSwarmReposition();
                }
            } else if (p < cmAOEProb) {
                if (prevState == EnemyState.AOEMeleeStartup) {
                    float pTotal = rotatingShotProbability + repositioningProbability + aoeProbability + missileProbability + summonProbability;
                    float q = Random.Range(cmAOEProb, pTotal + cmRepProb);
                    MakeDecision(q % totalProb);
                } else {
                    ChangeToAOEMeleeStartup();
                }
            } else if (p < cmMissileProb) {
                if (prevState == EnemyState.MissileAttack) {
                    float pTotal = rotatingShotProbability + repositioningProbability + aoeProbability + missileProbability + summonProbability;
                    float q = Random.Range(cmMissileProb, pTotal + cmAOEProb);
                    MakeDecision(q % totalProb);
                } else {
                    ChangeToMissileAttack();
                }
            } else {
                if (prevState == EnemyState.SummonAdds) {
                    float pTotal = rotatingShotProbability + repositioningProbability + aoeProbability + missileProbability + summonProbability;
                    float q = Random.Range(0, cmAOEProb);
                    MakeDecision(q % totalProb);
                } else {
                    ChangeToSummonAdds();
                }
            }
        }

        void ChangeToRotateShoot() {
            enemyState = EnemyState.RotateShooting;
            prevState = enemyState;
            rotatingShotTimer = Random.Range(rotatingShotTimerMin, rotatingShotTimerMax);
            changeBulletColTimer = changeBulletColTimerMax;
        }

        Vector3 GetTargetPosition(Vector3 pos) {
            RaycastHit hit;
            Vector3 playerDirection = Vector3.Normalize(pos - fireOrigin.position); //GetPlayerDirection();
            if (Physics.Raycast(fireOrigin.position, playerDirection, out hit, 999f, GlobalValues.Instance.environment | GlobalValues.Instance.playerLayer)) {
                return hit.point;
            } else {
                return fireOrigin.position;
            }
        }

        void RotateShoot() {
            currentGunAngle += (rotationSpeed * Time.deltaTime) % (Mathf.PI * 2);
            float phase = 0f;
            bool canShoot = false;
            bool changeCol = false;
            if (gunCooldownTimer <= 0) {
                gunCooldownTimer = gunCooldownTimerMax;
                canShoot = true;
            }
            if (changeBulletColTimer <= 0) {
                changeBulletColTimer = changeBulletColTimerMax;
                changeCol = true;
                switch (bulletColour) {
                    case (LightColour.Red):
                        bulletColour = LightColour.Green;
                        break;
                    case (LightColour.Green):
                        bulletColour = LightColour.Blue;
                        break;
                    case (LightColour.Blue):
                        bulletColour = LightColour.Red;
                        break;
                }
            }
            for (int i = 0; i < rotatingGuns.Count; i++) {

                EnemyGun g = rotatingGuns[i];
                if (changeCol) {
                    g.SetColour(bulletColour);
                }
                float x = transform.position.x + gunCircleRadius * Mathf.Cos(currentGunAngle + phase);
                float z = transform.position.z + gunCircleRadius * Mathf.Sin(currentGunAngle + phase);
                g.transform.position = new Vector3(x, g.transform.position.y, z);
                phase += (Mathf.PI * 2) / rotatingGuns.Count;
                if (canShoot) {
                    targetGOs[i].transform.position = new Vector3(transform.position.x + gunCircleRadius * 3 * Mathf.Cos(currentGunAngle + phase),
                    g.firePoint.position.y,
                    transform.position.z + gunCircleRadius * 3 * Mathf.Sin(currentGunAngle + phase));
                    g.SetTarget(targetGOs[i]);
                    g.Use();
                }
            }
            if (rotatingShotTimer <= 0) {
                enemyState = EnemyState.DecisionState;
            }
        }

        void ChangeToMissileAttack() {
            enemyState = EnemyState.MissileAttack;
            prevState = enemyState;
            missileShotsFired = 0;
            Debug.Log("start misiles");
        }

        void MissileAttack() {
            if (missileTimer <= 0) {
                Debug.Log("misiles");
                GameObject mis = PhotonNetwork.Instantiate("HomingMissile",transform.position,Quaternion.identity);
                MissileController mc = mis.GetComponent<MissileController>();
                mc.Fire(missileSpeed, missileTurnSpeed, missileDamage);
                missileShotsFired++;
                missileTimer = missileTimeBetweenShots; 
            }
            if (missileShotsFired >= missileShotsMax) {
                Debug.Log("no more misiles");
                enemyState = EnemyState.DecisionState;
            }
            
        }

        void Patrol() {

        }

        void ShowCircle(float time) {
            circleLR.enabled = true;
            DrawPolygon(100, aoeRadius, new Vector3(fireOrigin.position.x, 0.25f, fireOrigin.position.z), 0.1f, 0.1f);
            flashesRemaining = flashNum;
            flashTimerMax = time;
            flashTimer = flashTimerMax;
        }

        void ChangeToAOEMeleeStartup() {
            enemyState = EnemyState.AOEMeleeStartup;
            prevState = enemyState;
            aoeStartTimer = aoeStartTimerMax;
            agent.enabled = false;
            ShowCircle((aoeStartTimerMax) / (flashNum + 1));
        }

        void AOEMeleeStartup() {
            if (aoeStartTimer <= 0) {
                ChangeToAOEMelee();
            }
            //visual effect?
        }

        void ChangeToAOEMelee() {
            enemyState = EnemyState.AOEMelee;
        }

        void DoAOEAttack(float dmg) {
            Collider[] cols = Physics.OverlapSphere(transform.position, aoeRadius, GlobalValues.Instance.playerLayer);
            if (cols.Length > 0) {
                foreach (Collider col in cols) {
                    HealthSystem.Health h = col.gameObject.GetComponentInChildren<HealthSystem.Health>();
                    h.Damage(dmg, 0f);
                }
            }
        }

        void AOEMelee() {
            DoAOEAttack(aoeDamage);
            //do attack
            ChangeToAOEMeleeRecovery();
        }
        void ChangeToAOEMeleeRecovery() {
            circleLR.enabled = false;
            enemyState = EnemyState.AOEMeleeRecovery;
            aoeEndTimer = aoeEndTimerMax;
        }

        void AOEMeleeRecovery() {
            if (aoeEndTimer <= 0) {
                agent.enabled = true;
                enemyState = EnemyState.DecisionState;
            }
        }

        void ChangeToSummonAdds() {
            enemyState = EnemyState.SummonAdds;
            prevState = enemyState;
            summonTimer = summonTimerMax;
        }

        void SummonAdds() {
            if (summonTimer <= 0) {
                enemyState = EnemyState.DecisionState;
            }
            //do nothing
        }

        void ChangeToSwarmReposition() {
            enemyState = EnemyState.SwarmRepositioning;
            prevState = enemyState;
            NavMeshHit destPos;
            int index = Random.Range(0, GlobalValues.Instance.players.Count);
            playerObj = GlobalValues.Instance.players[index];
            NavMesh.SamplePosition(playerObj.transform.position, out destPos, 2f, NavMesh.AllAreas);
            agent.speed = repositionSpeed;
            agent.destination = destPos.position;
        }

        void SwarmReposition() {
            if (agent.remainingDistance <= pathStoppingThreshold) {
                ChangeToReappearState();
            }
        }

        void ChangeToReappearState() {
            enemyState = EnemyState.Reappearing;
            reappearingTimer = reappearingTimerMax;
            agent.speed = normalSpeed;
            agent.enabled = false;
            ShowCircle((reappearingTimerMax) / (flashNum + 1));
        }

        void ReappearState() {
            if (reappearingTimer <= 0) {
                DoAOEAttack(reappearDamage);
                circleLR.enabled = false;
                enemyState = EnemyState.DecisionState;
                agent.enabled = true;
            }
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
                if (summonTimer > 0) {
                    summonTimer -= Time.deltaTime;
                }
                if (gunCooldownTimer > 0) {
                    gunCooldownTimer -= Time.deltaTime;
                }
                if (changeBulletColTimer > 0) {
                    changeBulletColTimer -= Time.deltaTime;
                }
                if (movementCooldownTimer > 0) {
                    movementCooldownTimer -= Time.deltaTime;
                }
                if (flashTimer > 0) {
                    flashTimer -= Time.deltaTime;
                }
                if (missileTimer > 0) {
                    missileTimer -= Time.deltaTime;
                }
                yield return null;
            }
        }

        void DrawPolygon(int vertexNumber, float radius, Vector3 centerPos, float startWidth, float endWidth) {
            //https://www.codinblack.com/how-to-draw-lines-circles-or-anything-else-using-linerenderer/
            circleLR.startWidth = startWidth;
            circleLR.endWidth = endWidth;
            circleLR.loop = true;
            float angle = 2 * Mathf.PI / vertexNumber;
            circleLR.positionCount = vertexNumber;

            for (int i = 0; i < vertexNumber; i++) {
                Matrix4x4 rotationMatrix = new Matrix4x4(new Vector4(Mathf.Cos(angle * i), 0, 0, -1 * Mathf.Sin(angle * i)),
                                                        new Vector4(0, 1, 0, 0),
                                        new Vector4(Mathf.Sin(angle * i), 0, Mathf.Cos(angle * i), 0),
                                        new Vector4(0, 0, 0, 1));
                Vector3 initialRelativePosition = new Vector3(0, 0, radius);
                circleLR.SetPosition(i, centerPos + rotationMatrix.MultiplyPoint(initialRelativePosition));

            }
        }
    }
}
