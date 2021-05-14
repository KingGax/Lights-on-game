using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCollider : MonoBehaviour
{
    public int roomID;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("I AM A ROOM COLLIDER");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) {
        Debug.Log("ROOM COLLISION");
        GlobalValues.Instance.fm.UpdateLocation(other.gameObject, roomID);
    }
}
