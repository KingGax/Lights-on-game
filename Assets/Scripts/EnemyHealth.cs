using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable {
    public float health;
    // Start is called before the first frame update

    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void Damage(float damage) {
        health -= damage;
        if (health < 0) {
            Die();
        }
    }

    void Die() {
        Debug.Log("ded"); 
        Destroy(gameObject);
    }
}
