using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    float damage;
    int environmentLayer;
    int enemyLayer;
    public void Fire(float _damage)
    {
        damage = _damage;
    }

    // Start is called before the first frame update
    void Start()
    {
        environmentLayer = LayerMask.NameToLayer("Environment");
        enemyLayer = LayerMask.NameToLayer("Enemies");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == enemyLayer)
        {
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
