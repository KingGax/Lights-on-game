using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

    public List<GameObject> spawnableEntities;
    public float spawnRadius;
    int spawnIndex;


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
            spawnIndex = Random.Range(0, spawnableEntities.Count);
            GameObject entity = Instantiate(spawnableEntities[spawnIndex], pos, Quaternion.identity);
        }
    }
}