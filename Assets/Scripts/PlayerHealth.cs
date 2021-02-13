using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable {

    public float maxHealth;
    float health;

    // Start is called before the first frame update
    void Start() {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void Damage(float damage) {
        health -= damage;
        if (health <= 0) {
            Die();
        }
    }

    void Die() {
        Debug.Log("my healh has depleted to zero but there is not a game over scene aaaaaa");
    }
}