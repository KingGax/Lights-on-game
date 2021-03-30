using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SetSpawnManager : MonoBehaviour
{
    List<EnemyContainer> containers = new List<EnemyContainer>();
    float spawnTimer = 0f;
    int waveNumber = 0;
    int highestWaveNumber = 0;
    PhotonView pv;

    private void Awake() {
        pv = GetComponent<PhotonView>();
    }
    public void Initialise(Transform enemyContParent) {
        EnemyContainer[] contArr = enemyContParent.GetComponentsInChildren<EnemyContainer>();
        foreach (EnemyContainer cont in contArr) {
            containers.Add(cont);
            if (highestWaveNumber < cont.waveNumber) {
                highestWaveNumber = cont.waveNumber;
            }
        }
    }

    private void FixedUpdate() {
        spawnTimer -= Time.fixedDeltaTime;
    }

    public bool SpawnedAllWaves() {
        return waveNumber > highestWaveNumber;
    }

    public bool FinishedSpawning() {
        return spawnTimer <= 0;
    }

    public void SpawnWave(int waveNum, Transform enemyParent) {
        if (PhotonNetwork.IsMasterClient) {
            float maxSpawnTime = 0f;
            waveNumber = waveNum;
            foreach (EnemyContainer cont in containers) {
                if (cont.waveNumber == waveNum) {
                    cont.StartWave(waveNum, enemyParent);
                    if (cont.waveOffset > maxSpawnTime) {
                        maxSpawnTime = cont.waveOffset;
                    }
                }
            }
            spawnTimer = Mathf.Max(maxSpawnTime, 1.2f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
