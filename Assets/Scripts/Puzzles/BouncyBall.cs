using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using LightsOn.AudioSystem;

public class BouncyBall : MonoBehaviour {

    public Transform ball;
    private int staticEnvironmentMask = 1 << 9; //Mask to detect collision with static environment

    private int dynamicEnvironmentMask = 1 << 13; //Mask to detect collision with dynamic environment

    public float speed = 15;

    private int bouncesLeft = 4;

    private bool isActivated = false;

    private Rigidbody rigidBody;
    private int staticLayer;
    private int dynamicLayer;
    private LineRenderer lr;

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private PhotonView pv;

    //Set variables and values
    void Awake() {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        pv = GetComponent<PhotonView>();
        //rigidBody = this.gameObject.GetComponent<Rigidbody>();
        staticLayer = LayerMask.NameToLayer("StaticEnvironment");
        dynamicLayer = LayerMask.NameToLayer("DynamicEnvironment");
        lr = GetComponent<LineRenderer>();

    }

    //Places ball back to it's starting state
    public void Respawn() {
        if (!PhotonNetwork.IsMasterClient) return;
        pv.RPC("Deactivate", RpcTarget.Others);
        this.transform.position = spawnPosition;
        this.transform.rotation = spawnRotation;
        this.bouncesLeft = 4;
        this.isActivated = false;
        lr.positionCount = 0;
    }

    //Activates the ball and starts it's movement
    public void ActivateBall() {
        if (!PhotonNetwork.IsMasterClient || isActivated) return;
        this.isActivated = true;
        pv.RPC("Activate", RpcTarget.Others);
    }

    [PunRPC]
    private void Activate() {
        isActivated = true;
    }

    [PunRPC]
    private void Deactivate() {
        isActivated = false;
    }
    // Update moves the ball in the correct direction and determines whether there is a collision using raycasting
    // If there is a collision it adjusts the direction of movement or respawns the ball
    void FixedUpdate() {
        if (!PhotonNetwork.IsMasterClient) {
            if (isActivated) {
                transform.Translate(Vector3.forward * Time.fixedDeltaTime * speed);

                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hit;
                for (int i = 0; i < lr.positionCount; i++) {
                    lr.SetPosition(i, transform.position);
                }
                DrawLine(transform.forward, transform.position, 0);
            }
            return;
        }
        if (isActivated) {
            transform.Translate(Vector3.forward * Time.fixedDeltaTime * speed);

            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            for (int i = 0; i < lr.positionCount; i++) {
                lr.SetPosition(i, transform.position);
            }
            DrawLine(transform.forward, transform.position, 0);
            if (Physics.Raycast(ray, out hit, Time.fixedDeltaTime * speed + .1f, dynamicEnvironmentMask)) {
                //Reflect direcion and adjust rotation
                if (bouncesLeft == 0) {
                    AudioManager.Instance.PlaySFX(SoundClips.Instance.SFXBallShatter, transform.position, gameObject);
                    Respawn();
                    return;
                }
                Vector3 reflectDirection = Vector3.Reflect(ray.direction, hit.normal);
                float rotation = 90 - Mathf.Atan2(reflectDirection.z, reflectDirection.x) * Mathf.Rad2Deg;
                transform.eulerAngles = new Vector3(0, rotation, 0);
                AudioManager.Instance.PlaySFX(SoundClips.Instance.SFXBallBounce, transform.position, gameObject);
                bouncesLeft -= 1;
            } else if (Physics.Raycast(ray, out hit, Time.fixedDeltaTime * speed + .1f, staticEnvironmentMask)) {
                AudioManager.Instance.PlaySFX(SoundClips.Instance.SFXBallShatter, transform.position, gameObject);
                Respawn();
                return;
            }

        }
    }

    //Used to draw the visual ray of the ball's path
    private void DrawLine(Vector3 direction, Vector3 position, int depth) {
        if (depth > bouncesLeft) {
            return;
        }
        Ray ray = new Ray(position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, dynamicEnvironmentMask | staticEnvironmentMask)) {
            lr.positionCount = depth + 2;
            lr.SetPosition(depth + 1, hit.point);
            if (hit.collider.gameObject.layer == dynamicLayer) {
                Vector3 reflectDirection = Vector3.Reflect(ray.direction, hit.normal);
                DrawLine(reflectDirection, hit.point, depth + 1);
            } else {
                return;
            }

        }
    }

    //Destroys the ball upon achieving the puzzle objective
    public void DestroyBall() {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(this.gameObject);
    }

}


