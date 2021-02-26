using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class Spawner : MonoBehaviour {

    public List<GameObject> spawnableEntities;
    public float spawnRadius;
    public GameObject enemyParent;
    private LightableColour[] enemyColours = new LightableColour[]{ LightableColour.Red, LightableColour.Blue, LightableColour.Green };
    int spawnIndex;
    int colIndex;
    int spawnCount = 0;
    int waveSpawnNumber = 0;
    PhotonView pv;


    void Start() {
        pv = gameObject.GetPhotonView();
    }

    // Called at frame rate
    void Update() {}

    // Called at a fixed rate 50fps
    void FixedUpdate() {
        if (pv == null || !pv.IsMine) return;
        if (waveSpawnNumber > spawnCount && Random.value > 0.99) {
            Vector3 pos = transform.position
                + new Vector3(
                    spawnRadius * (Random.value * 2 - 1),
                    0, 
                    spawnRadius * (Random.value * 2 - 1)
                );
            spawnIndex = Random.Range(0, spawnableEntities.Count);
            colIndex = Random.Range(0, enemyColours.Length);
            GameObject entity = PhotonNetwork.Instantiate(spawnableEntities[spawnIndex].name, pos, Quaternion.identity);
            LightableEnemy lightScript = entity.GetComponentInChildren<LightableEnemy>();
            lightScript.InitialiseEnemy(enemyColours[colIndex]);
            spawnCount++;
        }
    }

    public void StartSpawning(int num)
    {
        spawnCount = 0;
        waveSpawnNumber = num;
    }

    public bool FinishedSpawning()
    {
        return spawnCount >= waveSpawnNumber;
    }
}