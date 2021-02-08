using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultWeapon : IGun
{
    public Transform firePoint;
    public GameObject bullet;
    public float bulletSpeed;
    public float damage;
    
    // Start is called before the first frame update
    void Start()
    {
        fireCooldown = 0;
        StartCoroutine("CountdownTimers");
    }

    public override bool RequestShoot(Vector3 direction)
    {
        bool canShoot = (fireCooldown <= 0); //conditions for shooting
        if (canShoot){
            Shoot(direction);
        }
        return canShoot;
    }

    public override void Shoot(Vector3 direction)
    {
        fireCooldown = fireCooldownMax;
        GameObject newBullet = Instantiate(bullet, firePoint.position, Quaternion.identity);
        BulletController bc = newBullet.GetComponent<BulletController>();
        bc.Fire(damage,bulletSpeed,direction);
        newBullet.transform.up = direction;
        Destroy(newBullet, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private IEnumerator CountdownTimers()
    {
        while (true)
        {
            if (fireCooldown > 0)
            {
                fireCooldown -= Time.deltaTime;
            }    
            yield return null;    
        }
    }
}
