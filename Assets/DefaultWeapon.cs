using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultWeapon : IGun
{
    public Transform firePoint;
    public GameObject bullet;
    public float bulletSpeed;
    public override void Shoot(Vector3 direction)
    {
        GameObject newBullet = Instantiate(bullet, firePoint.position, Quaternion.identity);
        newBullet.transform.up = direction;
        Rigidbody bulletRb = newBullet.GetComponent<Rigidbody>();
        bulletRb.velocity = direction.normalized * bulletSpeed;
        Destroy(newBullet, 2.0f);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
