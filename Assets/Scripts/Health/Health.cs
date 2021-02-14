using UnityEngine;

public class Health : MonoBehaviour, IDamageable {
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

    public virtual void Die() {
        Debug.Log("ded"); 
        Destroy(gameObject);
    }
}
