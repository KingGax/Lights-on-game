using UnityEngine;

public sealed class PlayerHealth : Health {

    public override void Die() {
        Debug.Log("my healh has depleted to zero but there is not a game over scene aaaaaa");
    }
}