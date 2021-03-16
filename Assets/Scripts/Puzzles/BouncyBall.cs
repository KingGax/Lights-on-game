using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyBall : MonoBehaviour
{

    public Transform ball;

    private float speed = 15;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Time.deltaTime * speed + .1f)){
            Vector3 reflectDirection = Vector3.Reflect(ray.direction,hit.normal);
            float rotation = 90 - Mathf.Atan2(reflectDirection.z,reflectDirection.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0,rotation,0);
        }   
    }
}
