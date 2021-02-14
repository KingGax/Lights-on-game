using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject spawnableEntity;
    public float spawnRadius;
    
    void Start() {}

    // Called at frame rate
    void Update() {}

    // Called at a fixed rate 25ups
    void FixedUpdate() {
        if (Random.value > 0.99) {
            Vector3 pos = transform.position
                + new Vector3(
                    spawnRadius * (Random.value * 2 - 1),
                    0, 
                    spawnRadius * (Random.value * 2 - 1)
                );
            GameObject entity = Instantiate(spawnableEntity, pos, Quaternion.identity);
        }
    }
}