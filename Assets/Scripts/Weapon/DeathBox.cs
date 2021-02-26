using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBox : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        Health ds = other.gameObject.GetComponent<Health>();
        if (ds != null) {
            ds.Damage(9999999);
        }
    }
}
