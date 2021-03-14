using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScript : MonoBehaviour {
    int playerLayer;
    private void Start() {
        playerLayer = LayerMask.NameToLayer("Player");
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == playerLayer) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
