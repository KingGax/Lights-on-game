using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LevelManager : MonoBehaviour
{
    public GameObject door;
    public GameObject enemyParent;
    public Spawner spawnScript;
    public float spawnTime;
    private PhotonView pv;
    private LightableObject doorLightable;
    private bool allEnemiesSpawned = false;
    // Start is called before the first frame update
    void Start()
    {
        pv = gameObject.GetPhotonView();
        Invoke("DisableSpawner", spawnTime);
        doorLightable = door.GetComponentInChildren<LightableObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pv == null || !pv.IsMine) return;
        if (allEnemiesSpawned)
        {
            if (enemyParent.transform.childCount == 0)
            {
                doorLightable.enabled = true;
            }
        }
    }

    void DisableSpawner()
    {
        spawnScript.enabled = false;
        allEnemiesSpawned = true;
    }
}
