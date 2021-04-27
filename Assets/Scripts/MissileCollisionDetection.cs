using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileCollisionDetection : MonoBehaviour
{
    MissileController missile;
    // Start is called before the first frame update
    void Start()
    {
        missile = transform.parent.GetComponent<MissileController>();
    }


    // Update is called once per frame
    void Update()
    {
        
    }


}
