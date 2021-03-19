using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleDoorTrigger : MonoBehaviour
{
    // Start is called before the first frame update

    public LightableExitDoor door;

    void OnTriggerEnter(Collider other)
    {

        //Debug.Log(other.gameObject.layer);
        if(other.gameObject.layer == 17){
            door.UnlockDoor();
            other.transform.gameObject.GetComponent<BouncyBall>().DestroyBall();
        }
    }
}
