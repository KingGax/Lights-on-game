using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemySpawnInitialiser : MonoBehaviour
{
    public PhotonView pv;
    private PhotonView[] childPVs;
    public GameObject enemy;
    private double spawnTime = double.MaxValue;
    private LightableColour enemyCol;
    private string parentStr;
    bool spawned = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!spawned && PhotonNetwork.Time > spawnTime) {
            spawned = true;
            SpawnEnemy();
        }
        else {
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

    public void SetupSpawner(LightableColour col, double _spawnTime, string parentString) {
        pv.RPC("SetupSpawnerRPC", RpcTarget.AllBufferedViaServer, col, _spawnTime, parentString);
    }

    [PunRPC]
    public void SetupSpawnerRPC(LightableColour col, double _spawnTime, string parentString) {
        parentStr = parentString;
        transform.parent.SetParent(GameObject.Find(parentStr).transform);
        spawnTime = _spawnTime;
        enemyCol = col;
    }

    void SpawnEnemy() {
        LightableEnemy enemyScript = enemy.GetComponentInChildren<LightableEnemy>();
        childPVs = enemy.GetComponentsInChildren<PhotonView>();
        enemy.SetActive(true);
        if (enemy != null) {
            enemyScript.InitialiseEnemy(enemyCol, parentStr);
        }
    }
}
