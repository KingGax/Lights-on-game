using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyBall : MonoBehaviour
{

    public Transform ball;

    //private int layerMask = 0x00000200; 
    private int staticEnvironmentMask = 1 << 9;

    private int dynamicEnvironmentMask = 1 << 13;

    private float speed = 15;

    private int bouncesLeft = 3;

    private bool isActivated = false;

    private Rigidbody rigidBody;

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    // Start is called before the first frame update

    void Awake()
    {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        rigidBody = this.gameObject.GetComponent<Rigidbody>();
    }
    void Start()
    {
        
        
    }

    void Respawn()
    {
        this.transform.position = spawnPosition;
        this.transform.rotation = spawnRotation;
        this.bouncesLeft = 3;
        this.isActivated = false;
        rigidBody.velocity = Vector3.zero;
    }

    public void ActivateBall()
    {
        Debug.Log("activated");
        this.isActivated = true;
        rigidBody.velocity = transform.forward.normalized * speed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isActivated)
        {
            //transform.Translate(Vector3.forward * Time.deltaTime * speed);

            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Time.fixedDeltaTime * speed + 2, dynamicEnvironmentMask)){
                //Reflect direcion and adjust rotation
                if(bouncesLeft == 0){
                    Respawn();
    
                }
                Vector3 reflectDirection = Vector3.Reflect(ray.direction,hit.normal);
                float rotation = 90 - Mathf.Atan2(reflectDirection.z,reflectDirection.x) * Mathf.Rad2Deg;
                transform.eulerAngles = new Vector3(0,rotation,0);
                rigidBody.velocity = reflectDirection.normalized * speed;

                bouncesLeft -= 1;
            } else if (Physics.Raycast(ray, out hit, Time.fixedDeltaTime * speed + .1f, staticEnvironmentMask)){
                Respawn();
            }
        }
    }

    public void DestroyBall(){
        Destroy(this.gameObject);
    }

}


