using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using LightsOn.LightingSystem;

public class EnemySpawnInitialiser : MonoBehaviour {
    public PhotonView pv;
    private PhotonView[] childPVs;
    public GameObject enemy;
    private double spawnTime = double.MaxValue;
    private LightColour enemyCol;
    public GameObject spawnDecal;
    private string parentStr;
    bool spawned = false;
    bool animSpawned = false;
    int hiddenEnemyLayer;
    const double animTime = 1 + 1/3;
    public float spawnDecalYOffset = 0.0f;

    private void Awake() {
        hiddenEnemyLayer = LayerMask.NameToLayer("HiddenEnemies");
    }

    void Update() {
        if (!animSpawned && PhotonNetwork.Time > spawnTime) { 
            animSpawned = true;
            SpawnAnim();
        }
        if (!spawned && PhotonNetwork.Time > spawnTime + animTime) {
            spawned = true;
            SpawnEnemy();
        } else {
            if (enemy == null) {
                foreach (PhotonView childPV in childPVs) {
                    if (childPV.IsMine) {
                        PhotonNetwork.CleanRpcBufferIfMine(childPV);
                    }
                }

                if (pv.IsMine) {
                    PhotonNetwork.CleanRpcBufferIfMine(pv);
                    PhotonNetwork.Destroy(transform.parent.gameObject);
                }
            }
        }
    }

    public void SetupSpawner(LightColour col, double _spawnTime, string parentString) {
        pv.RPC("SetupSpawnerRPC", RpcTarget.AllBufferedViaServer, col, _spawnTime, parentString);
    }

    [PunRPC]
    public void SetupSpawnerRPC(LightColour col, double _spawnTime, string parentString) {
        parentStr = parentString;
        transform.parent.SetParent(GameObject.Find(parentStr).transform);
        spawnTime = _spawnTime;
        enemyCol = col;
    }

    protected virtual void SpawnAnim() {
        Vector3 decalPos = new Vector3(transform.position.x, transform.position.y + spawnDecalYOffset, transform.position.z);
        Instantiate(spawnDecal, decalPos, Quaternion.identity);
    }

    void SpawnEnemy() {
        LightableEnemy enemyScript = enemy.GetComponentInChildren<LightableEnemy>();
        childPVs = enemy.GetComponentsInChildren<PhotonView>();
        enemy.layer = hiddenEnemyLayer;
        enemy.SetActive(true);
        if (enemy != null) {
            enemyScript.InitialiseEnemy(enemyCol, parentStr);
        }
    }
}
