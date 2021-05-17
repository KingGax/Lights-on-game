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
    int[] numEnemiesInFutureWaves;
    public GameObject enemyContainers;
    public GameObject enemyParent;
    ObjectiveTextManager objectiveText;
    bool started = false;
    int prevNumEnemies = -1;

    void Start() {
        objectiveText = GlobalValues.Instance.UIElements.GetComponentInChildren<ObjectiveTextManager>();
        spawnScript = gameObject.AddComponent<SetSpawnManager>();
        pv = gameObject.GetPhotonView();
        spawnScript.Initialise(enemyContainers.transform);
        numEnemiesInFutureWaves = spawnScript.GetWaveSums();
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
                        pv.RPC("UpdateObjectiveText", RpcTarget.AllBufferedViaServer, 0);
                    }
                }
            } else {
                if (WaveSpawnFinished() && CountEnemies() == 0) {
                    StartNewSetWave();
                }
            }

            if (currentWaveCounter < numEnemiesInFutureWaves.Length) {
                if (prevNumEnemies != GetEnemiesLeft()) {
                    prevNumEnemies = GetEnemiesLeft();
                    pv.RPC("UpdateObjectiveText", RpcTarget.AllBufferedViaServer, prevNumEnemies);
                }
            } else {
                if (!complete){
                    pv.RPC("SetCompleteTrue", RpcTarget.All);
                }                
            }
        }
    }

    [PunRPC]
    void UpdateObjectiveText(int prevEnemies) {
        objectiveText.RefreshEnemyObjective(prevEnemies);
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
        } else {
            spawnScript.SpawnWave(currentWaveCounter, enemyParent.transform);
        }
    }

    public int GetEnemiesLeft() {
        if (currentWaveCounter < numEnemiesInFutureWaves.Length) {
            return CountEnemies() + numEnemiesInFutureWaves[currentWaveCounter];
        } else {
            complete = true;
            return 0;
        }
        
    }

    int CountEnemies() {
        return enemyParent.transform.childCount;
    }

    bool WaveSpawnFinished() {
        return spawnScript.FinishedSpawning();
    }
}
