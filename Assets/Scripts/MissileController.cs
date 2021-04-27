using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class MissileController : MonoBehaviour
{
    // Start is called before the first frame update
    private float moveSpeed = 0;
    public int framesPerRetarget;
    public int damage;
    private float turnSpeed;
    private int retargetCounter;
    GameObject targetPlayer;
    PhotonView pv;
    bool started = false;
    bool fireOnStart = false;
    public void Fire(float speed,float _turnSpeed, int _damage) {
        moveSpeed = speed;
        turnSpeed = _turnSpeed;
        damage = _damage;
        if (started) {
            pv.RPC("FireRPC", RpcTarget.All, speed, _turnSpeed,_damage);
        } else {
            fireOnStart = true;
        }
        
    }
    [PunRPC]
    private void FireRPC(float speed, float _turnSpeed, int _damage) {
        moveSpeed = speed;
        turnSpeed = _turnSpeed;
        damage = _damage;
    }
    public void RequestDestroy() {
        pv.RPC("RequestDestroyRPC", RpcTarget.All);
    }

    [PunRPC]
    private void RequestDestroyRPC() {
        if (pv.IsMine) {
            Detonate();
        }
    }


    void Start()
    {
        retargetCounter = framesPerRetarget;
        pv = GetComponent<PhotonView>();
        SelectTarget();
        if (fireOnStart) {
            pv.RPC("FireRPC", RpcTarget.All, moveSpeed, turnSpeed, damage);
        }
    }


    public void Detonate() {
        
        if (pv == null || !pv.IsMine) return;
        PhotonNetwork.Destroy(gameObject);
    }

    protected int SelectTarget() { //default implementation sets target as closest player
        float minDist = Mathf.Infinity;
        int targetIndex = 0;
        for (int i = 0; i < GlobalValues.Instance.players.Count; i++) {
            float distToPlayer = Vector3.Distance(gameObject.transform.position, GlobalValues.Instance.players[i].transform.position);
            if (distToPlayer < minDist) {
                //Debug.Log("Player distance: "+ distToPlayer);
                minDist = distToPlayer;
                targetIndex = i;
            }
        }
        //Debug.Log("Player index: "+targetIndex);
        targetPlayer = GlobalValues.Instance.players[targetIndex];
        return targetIndex;
    }


    // Update is called once per frame
    void Update()
    {
        if (pv == null || !pv.IsMine) return;
        if (retargetCounter == 0) {
            retargetCounter = framesPerRetarget;
            SelectTarget();
        }
        transform.up = Vector3.RotateTowards(transform.up, targetPlayer.transform.position - transform.position,Time.deltaTime*turnSpeed,0);
        transform.position += transform.up * moveSpeed * Time.deltaTime;
        retargetCounter--;   
    }
}
