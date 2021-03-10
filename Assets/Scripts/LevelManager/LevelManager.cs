using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Wave {
    public Transform centre;
    public float spawnRate;
    public int numEnemies;
}

public class LevelManager : MonoBehaviour {
    public GameObject door;
    public SetSpawnManager spawnScript;
    public float spawnTime;
    public GameObject whiteLight;
    public GameObject magentaLight;
    public GameObject EndTooltip;
    public List<int> enemyWaveNumbers;
    public List<Transform> wavePositions;
    public float spawnRate;
    public List<Wave> waves;
    private GameObject enemyParent;
    private PhotonView pv;
    private LightableObject doorLightable;
    private bool allEnemiesSpawned = false;
    private bool allWavesSpawned = false;
    bool doorUnlocked = false;
    private int currentWaveCounter = -1;
    public int NumberOfWaves;
    public LightableExitDoor exit;
    public List<LightableExitDoor> entrances;
    bool started = false;

    void Start() {
        enemyParent = GlobalValues.Instance.enemyParent;
        pv = gameObject.GetPhotonView();
        if (pv == null || !pv.IsMine) spawnScript.enabled = false;
        spawnScript.Initialise();
        
    }

    public void StartLevel() {
        started = true;
        StartNewSetWave();
    }

    [PunRPC]
    public void UnlockDoor() {
        exit.UnlockDoor();
        EndTooltip.SetActive(false);
    }

    public void LockEntrances() {
        foreach (LightableExitDoor door in entrances) {
            door.LockDoor();
        }
    }

   [PunRPC]
   private void UnlockLocalEntrances() {
        foreach (LightableExitDoor door in entrances) {
            door.UnlockDoor();
        }
    }

    private void UnlockEntrances() {
        pv.RPC("UnlockLocalEntrances", RpcTarget.All);
    }

    void Update() {
        if (pv == null || !pv.IsMine) return;
        if (started) {
            if (allWavesSpawned) {
                if (CountEnemies() == 0) {
                    if (!doorUnlocked) {
                        UnlockEntrances();
                        pv.RPC("UnlockDoor", RpcTarget.All);
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
        for (int i = currentWaveCounter; i < enemyWaveNumbers.Count; i++)
            left += enemyWaveNumbers[i];
        Tooltip t = EndTooltip.GetComponent<Tooltip>();
        t.Text = "Defeat " + left + " more enemies";
    }

    /*void StartNewWave() {  
        currentWaveCounter += 1;
        if (currentWaveCounter == NumberOfWaves) {
            allWavesSpawned = true;
        }
        else {
            Wave currentWave = new Wave();
            currentWave.centre = wavePositions[currentWaveCounter];
            currentWave.numEnemies = enemyWaveNumbers[currentWaveCounter];
            currentWave.spawnRate = spawnRate;
            spawnScript.SpawnWave(currentWave);
        }
    }*/

    void StartNewSetWave() {
        currentWaveCounter += 1;
        if (spawnScript.SpawnedAllWaves()) {
            allWavesSpawned = true;
        }
        else {
            spawnScript.SpawnWave(currentWaveCounter);
        }
    }

    int CountEnemies() {
        return enemyParent.transform.childCount;
    }

    bool WaveSpawnFinished() {
        return spawnScript.FinishedSpawning();
    }
}
