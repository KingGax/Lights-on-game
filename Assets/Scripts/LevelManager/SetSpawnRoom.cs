using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Wave {
    public Transform centre;
    public float spawnRate;
    public int numEnemies;
}

public class SetSpawnRoom : RoomObjective {
    private SetSpawnManager spawnScript;
    private bool allWavesSpawned = false;
    bool doorUnlocked = false;
    private int currentWaveCounter = -1;
    
    public GameObject enemyContainers;
    public GameObject enemyParent;
    bool started = false;

    void Start() {
        spawnScript = gameObject.AddComponent<SetSpawnManager>();
        pv = gameObject.GetPhotonView();
        spawnScript.Initialise(enemyContainers.transform);
    }

    [PunRPC]
    void SetStartedRPC(bool _started) {
        started = _started;
    }

    public override void StartObjective() {
        started = true;
        pv.RPC("SetStartedRPC", RpcTarget.AllBufferedViaServer, true);
        LockEntrancesGlobal();
        LockExitGlobal();
        StartNewSetWave();
    }


    void Update() {
        if (pv == null || !pv.IsMine) return;
        if (started) {
            if (allWavesSpawned) {
                if (CountEnemies() == 0) {
                    if (!doorUnlocked) {
                        UnlockEntrancesGlobal();
                        UnlockExitGlobal();
                        doorUnlocked = true;
                    }
                }
            }
            else {
                if (WaveSpawnFinished() && CountEnemies() == 0) {
                    StartNewSetWave();
                }
            }

            UpdateEndGameTooltip();
        }
    }

    private void UpdateEndGameTooltip() {
        int left = CountEnemies();
        // Tooltip t = EndTooltip.GetComponent<Tooltip>();
        // t.Text = "Defeat " + left + " more enemies";
    }

    [PunRPC]
    void SetWaveCounterRPC(int num) {
        currentWaveCounter = num;
    }

    void StartNewSetWave() {
        currentWaveCounter += 1;
        pv.RPC("SetWaveCounterRPC", RpcTarget.AllBufferedViaServer, currentWaveCounter);
        if (spawnScript.SpawnedAllWaves()) {
            allWavesSpawned = true;
        }
        else {
            spawnScript.SpawnWave(currentWaveCounter, enemyParent.transform);
        }
    }

    int CountEnemies() {
        return enemyParent.transform.childCount;
    }

    bool WaveSpawnFinished() {
        return spawnScript.FinishedSpawning();
    }
}
