using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightsOn.WeaponSystem;
using Photon.Pun;

public class StartBossFight : AfterDialogue
{

    [SerializeField]
    private BossController boss = null;

    [SerializeField]
    private int targetActivations;

    private int activations = 0;

    [SerializeField]
    private bool isEndlessMode = false;

    void Start(){
        StartCoroutine(GetBossObject());
        
    }

    private IEnumerator GetBossObject(){
        GameObject spawnedBoss = null;
        while(spawnedBoss == null){
            spawnedBoss = GameObject.Find("SpawnedBossEnemy");
            yield return new WaitForSeconds(1);
        }
        boss = spawnedBoss.GetComponent<BossController>();
        if(PhotonNetwork.PlayerList.Length == 1 || isEndlessMode){
            ActivateEffect();
        }
    } 
    
    public override void Effect(){
        activations++;
        PhotonView pv = this.GetComponent<PhotonView>();
        pv.RPC("Increment", RpcTarget.Others);
        if(activations == targetActivations) ActivateEffect();
    }
    [PunRPC]
    private void Increment(){
        activations++;
        if(activations == targetActivations) ActivateEffect();
    }

    private void ActivateEffect(){
        boss.Activate();
    }
}
