using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScript : MonoBehaviour
{
    int playerLayer;
    private void Start() {
        playerLayer = LayerMask.NameToLayer("Player");
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == playerLayer) {
            //TODO implement win popup
            Debug.Log("Win popup");
        }
    }
}
