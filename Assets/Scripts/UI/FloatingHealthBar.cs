using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingHealthBar : HealthBar
{
    Transform parentTransform;
    // Start is called before the first frame update
    void Start()
    {
        parentTransform = gameObject.GetComponentInParent<Transform>();        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, -parentTransform.rotation.y-148.5f, 0);
        //transform.LookAt(Camera.main.transform.position);
        //transform.rotation = Quaternion.Euler(0f, transform.rotation.y, 0f);
    }
}
