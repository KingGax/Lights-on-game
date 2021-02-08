using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // Start is called before the first frame update
    void Start()
    {
        //rb = gameObject.GetComponent<Rigidbody>();
        playerObj = GameObject.Find("Player");
        canShoot = true;
        StartCoroutine("EnemyTimers");
    }

    // Update is called once per frame
    void Update()
    {
        float distToPlayer = Vector3.Distance(playerObj.transform.position, transform.position);
        if (distToPlayer <= detectionThreshold && canShoot){
            Vector3 targetVector = playerObj.transform.position-firePoint.position;
            Shoot(targetVector);
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
            yield return null;
        }
    }
}
