using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LevelManager : MonoBehaviour {
    public GameObject door;
    public Spawner spawnScript;
    public float spawnTime;
    public List<int> enemyWaveNumbers;
    private GameObject enemyParent;
    private PhotonView pv;
    private LightableObject doorLightable;
    private bool allEnemiesSpawned = false;
    private bool allWavesSpawned = false;
    private int currentWaveCounter = -1;
    // Start is called before the first frame update
    void Start() {
        enemyParent = GlobalValues.Instance.enemyParent;
        pv = gameObject.GetPhotonView();
        doorLightable = door.GetComponentInChildren<LightableObject>();
        if (pv == null || !pv.IsMine) spawnScript.enabled = false;
        StartNewWave();
    }

    // Update is called once per frame
    void Update() {
        if (pv == null || !pv.IsMine) return;
        if (allWavesSpawned) {
            if (CountEnemies() == 0) {
                doorLightable.enabled = true;
            }
        }
        else {
            if (WaveSpawnFinished() && CountEnemies() == 0) {
                StartNewWave();
            }
        }
    }

    void StartNewWave() {
        currentWaveCounter += 1;
        if (currentWaveCounter == enemyWaveNumbers.Count) {
            allWavesSpawned = true;
        }
        else {
            spawnScript.StartSpawning(enemyWaveNumbers[currentWaveCounter]);
        }
    }

    int CountEnemies() {
        return enemyParent.transform.childCount;
    }

    bool WaveSpawnFinished() {
        return spawnScript.FinishedSpawning();
    }
}
