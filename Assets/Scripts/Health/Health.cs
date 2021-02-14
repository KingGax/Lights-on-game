using UnityEngine;

public abstract class Health : MonoBehaviour, IDamageable {
    public float maxHealth;
    private float health;

    void Start() {
        health = maxHealth;
    }

    void Update() {
        
    }

    public void Damage(float damage) {
        health -= damage;
        if (health < 0) {
            Die();
        }
    }

    public abstract void Die();
}
