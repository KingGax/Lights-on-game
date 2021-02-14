using UnityEngine;

public class EntityHealth : Health {

    public override void Die() {
        Debug.Log("ded"); 
        Destroy(gameObject);
    }
}