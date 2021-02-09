using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    
    Rigidbody rb;
    GameObject playerObj;
    public GameObject bullet;
    public float damage;
    public float bulletSpeed;
    public float detectionThreshold;
    public float fireCooldownMax;
    public Transform firePoint;
    float fireCooldown;
    bool canShoot;    
    NavMeshAgent agent;
    public float shootingTimerMax;
    public float engageDistance;
    float shootingTimer;
    
    EnemyState enemyState;
    enum EnemyState{
        Shooting, //Actively attacking the player
        Patrolling, //Moving/idle state - hasn't engaged the player yet
        Repositioning //Moving during combat
    }

    // Start is called before the first frame update
    void Start()
    {
        //rb = gameObject.GetComponent<Rigidbody>();
        playerObj = GameObject.Find("Player");
        canShoot = true;
        StartCoroutine("EnemyTimers");
        agent = GetComponent<NavMeshAgent>();
        enemyState = EnemyState.Patrolling;
    }

    // Update is called once per frame
    void Update()
    {
        switch (enemyState)
        {
            case EnemyState.Patrolling:
                Patrol();
                break;
            case EnemyState.Repositioning:
                Repositioning();
                break;
            case EnemyState.Shooting:
                Shooting();
                break;
            default:
                break;
        }
       
    }
    void Patrol()
    {
        float distToPlayer = Vector3.Distance(playerObj.transform.position, transform.position);
        if (distToPlayer < detectionThreshold)
        {
            ChangeToRepositioning();
        }
    }
    void ChangeToShooting()
    {
        agent.enabled = false;
        shootingTimer = shootingTimerMax;
        enemyState = EnemyState.Shooting;
    }
    void Shooting()
    {
        float distToPlayer = Vector3.Distance(playerObj.transform.position, transform.position);
        if (distToPlayer <= detectionThreshold && canShoot)
        {
            Vector3 targetVector = playerObj.transform.position - firePoint.position;
            Shoot(targetVector);
        }
        if (shootingTimer <= 0)
        {
            ChangeToRepositioning();
        }
    }
    void ChangeToRepositioning()
    {
        enemyState = EnemyState.Repositioning;
        agent.enabled = true;
    }
    void Repositioning()
    {
        float distToPlayer = Vector3.Distance(playerObj.transform.position, transform.position);
        
        if (distToPlayer <= engageDistance)
        {
            ChangeToShooting();
        }
        else
        {
            agent.destination = playerObj.transform.position;
        }
    }
    public void Shoot(Vector3 direction)
    {
        fireCooldown = fireCooldownMax;
        canShoot = false;
        GameObject newBullet = Instantiate(bullet, firePoint.position, Quaternion.identity);
        BulletController bc = newBullet.GetComponent<BulletController>();
        bc.Fire(damage, bulletSpeed,direction);
        Destroy(newBullet, 2.0f);
    }

    private IEnumerator EnemyTimers()
    {
        while (true)
        {
            if (fireCooldown > 0)
            {
                fireCooldown -= Time.deltaTime;
                if (fireCooldown <= 0)
                {
                    canShoot = true;  
                }
            }
            if (shootingTimer > 0)
            {
                shootingTimer -= Time.deltaTime;
                
            }
            yield return null;
        }
    }
}
