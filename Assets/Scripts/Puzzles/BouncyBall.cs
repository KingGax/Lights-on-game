using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BouncyBall : MonoBehaviour
{

    public Transform ball;

    //private int layerMask = 0x00000200; 
    private int staticEnvironmentMask = 1 << 9;

    private int dynamicEnvironmentMask = 1 << 13;

    public float speed = 15;

    private int bouncesLeft = 4;

    private bool isActivated = false;

    private Rigidbody rigidBody;

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    // Start is called before the first frame update

    void Awake()
    {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        //rigidBody = this.gameObject.GetComponent<Rigidbody>();

        Debug.Log("awake bouncy ball");
    }
    void Start()
    {
        
        
    }

    public void Respawn()
    {
        if(!PhotonNetwork.IsMasterClient) return;
        this.transform.position = spawnPosition;
        this.transform.rotation = spawnRotation;
        this.bouncesLeft = 4;
        this.isActivated = false;
        //rigidBody.velocity = Vector3.zero;
    }

    public void ActivateBall()
    {
        if(!PhotonNetwork.IsMasterClient || isActivated) return;
        Debug.Log("activated");
        this.isActivated = true;
        Debug.Log(transform.forward.normalized * speed);
        //rigidBody.velocity = transform.forward.normalized * speed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!PhotonNetwork.IsMasterClient) return;
        if(isActivated)
        {
            transform.Translate(Vector3.forward * Time.fixedDeltaTime * speed);

            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Time.fixedDeltaTime * speed + .1f, dynamicEnvironmentMask)){
                //Reflect direcion and adjust rotation
                if(bouncesLeft == 0){
                    Respawn();
                    return;
                }
                Vector3 reflectDirection = Vector3.Reflect(ray.direction,hit.normal);
                float rotation = 90 - Mathf.Atan2(reflectDirection.z,reflectDirection.x) * Mathf.Rad2Deg;
                transform.eulerAngles = new Vector3(0,rotation,0);
                //rigidBody.velocity = reflectDirection.normalized * speed;
                Debug.Log("bounce");

                bouncesLeft -= 1;
            } else if (Physics.Raycast(ray, out hit, Time.fixedDeltaTime * speed + .1f, staticEnvironmentMask)){
                Debug.Log("Hit static wall");
                Respawn();
                return;
            }
        }
    }

    public void DestroyBall(){
        if(PhotonNetwork.IsMasterClient)
        PhotonNetwork.Destroy(this.gameObject);
    }

}


