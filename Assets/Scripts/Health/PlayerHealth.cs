using UnityEngine;
using Photon.Pun;

public sealed class PlayerHealth : Health {

    int bulletLayer;
    public override void Start() {
        base.Start();
        HealthBar hb = GlobalValues.Instance.UIElements.GetComponentInChildren<HealthBar>();
        hb.SetPlayer();
        hb.UpdateMaxHealth(maxHealth);
        hb.UpdateHealth(health);
    }

    public override void Awake() {
        base.Awake();
        bulletLayer = LayerMask.NameToLayer("EnemyBullets");
        
        
    }

    public override void Die() {
        Debug.Log("my healh has depleted to zero but there is not a game over scene aaaaaa");
    }

    public override void Damage(float damage)
    {
        base.Damage(damage);
        HealthBar hb = GlobalValues.Instance.UIElements.gameObject.GetComponentInChildren<HealthBar>();
        hb.UpdateHealth(health);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == bulletLayer && pv.IsMine) {
            BulletController bc = other.gameObject.GetComponent<BulletController>();
            if (bc != null) {
                Damage(bc.damage);
                bc.RequestDestroyBullet();
            } else {
                Debug.LogError("Something on bullet layer does not have bullet controller");
            }
        }
    }
}