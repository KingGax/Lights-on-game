using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TwoPlayerSetSpawnRoom : RoomObjective
{
    private SetSpawnManager spawnScriptP1;
    private SetSpawnManager spawnScriptP2;
    private bool allWavesSpawnedP1 = false;
    private bool allWavesSpawnedP2 = false;
    bool doorUnlocked = false;
    private int currentWaveCounter = -1;

    public GameObject enemyContainersP1;
    public GameObject enemyParentP1;
    public GameObject enemyContainersP2;
    public GameObject enemyParentP2;
    bool twoPlayers = false;
    bool started = false;
    // Start is called before the first frame update
    void Start()
    {
        spawnScriptP1 = gameObject.AddComponent<SetSpawnManager>();
        spawnScriptP2 = gameObject.AddComponent<SetSpawnManager>();
        pv = gameObject.GetPhotonView();
        spawnScriptP1.Initialise(enemyContainersP1.transform);
        spawnScriptP2.Initialise(enemyContainersP2.transform);
    }

    public override void StartObjective() {
        if (pv == null || !pv.IsMine) return;
        started = true;
        pv.RPC("SetStartedRPC", RpcTarget.AllBufferedViaServer, true);
        if (GlobalValues.Instance.players.Count == 2) {
            pv.RPC("SetTwoPlayersRPC", RpcTarget.AllBufferedViaServer, true);
            twoPlayers = true;
        }
        else {
            allWavesSpawnedP2 = true;
        }
        LockEntrancesGlobal();
        LockExitGlobal();
        StartNewSetWave();
    }

    [PunRPC]
    void SetTwoPlayersRPC(bool _twoPlayers) {
        twoPlayers = _twoPlayers;
    }

    [PunRPC]
    void SetStartedRPC(bool _started) {
        started = _started;
    }


    [PunRPC]
    void SetWaveCounterRPC(int num) {
        currentWaveCounter = num;
    }

    void Update() {
        if (pv == null || !pv.IsMine) return;
        if (started) {
            if (allWavesSpawnedP1 && allWavesSpawnedP2) {
                if (CountEnemies() == 0) {
                    if (!doorUnlocked) {
                        UnlockEntrancesGlobal();
                        UnlockExitGlobal();
                        pv.RPC("SetCompleteTrue", RpcTarget.AllBufferedViaServer);
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
        //Tooltip t = EndTooltip.GetComponent<Tooltip>();
       // t.Text = "Defeat " + left + " more enemies";
    }

    void StartNewSetWave() {
        currentWaveCounter += 1;
        pv.RPC("SetWaveCounterRPC", RpcTarget.AllBufferedViaServer, currentWaveCounter);
        if (twoPlayers) {
            if (spawnScriptP2.SpawnedAllWaves()) {
                allWavesSpawnedP2 = true;
            }
            else {
                spawnScriptP2.SpawnWave(currentWaveCounter, enemyParentP2.transform);
            }
        }
        if (spawnScriptP1.SpawnedAllWaves()) {
            allWavesSpawnedP1 = true;
        }
        else {
            spawnScriptP1.SpawnWave(currentWaveCounter, enemyParentP1.transform);
        }
        
    }

    int CountEnemies() {
        return enemyParentP1.transform.childCount + enemyParentP2.transform.childCount;
    }

    bool WaveSpawnFinished() {
        if (twoPlayers) {
            return spawnScriptP2.FinishedSpawning() && spawnScriptP1.FinishedSpawning();
        }
        return spawnScriptP1.FinishedSpawning();
    }
}
