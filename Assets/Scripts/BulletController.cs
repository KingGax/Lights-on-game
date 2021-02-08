using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    float damage;
    float speed;
    Vector3 direction;
    int environmentLayer;
    int enemyLayer;
    Rigidbody rb;
    public bool playerBullets;
    
    public void Fire(float _damage, float _speed, Vector3 _direction)
    {
        damage = _damage;
        direction = _direction;
        speed = _speed;
        rb.velocity = direction.normalized * speed;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        environmentLayer = LayerMask.NameToLayer("Environment");
        if (playerBullets)
        {
            enemyLayer = LayerMask.NameToLayer("Enemies");
        }
        else
        {
            enemyLayer = LayerMask.NameToLayer("Player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == enemyLayer)
        {
            Debug.Log(other);
            IDamageable damageScript = other.gameObject.GetComponent<IDamageable>();
            if (damageScript != null)
            {
                damageScript.Damage(damage);
                DestroyBullet();
            }
        }
        else if (other.gameObject.layer == environmentLayer)
        {
            DestroyBullet();
        }
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
