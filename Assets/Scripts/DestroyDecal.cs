using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDecal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DestroySpawnDecal() {
        Debug.Log("Destroy decal");
        Destroy(gameObject);
    }
}