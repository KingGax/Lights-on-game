using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    public float velX = 0.5f;
    private float pitch;
    private float yaw;
    public float velY = 0.5f;
    public bool rot = true;
    private float originPitch, originYaw;
    private Vector3 pos, originPos;
    // Start is called before the first frame update
    void Start()
    {
        pitch = transform.eulerAngles[0];
        yaw = transform.eulerAngles[1];
        originPitch = pitch;
        originYaw = yaw;
        pos = new Vector3(transform.position[0], transform.position[1], transform.position[2]);
        originPos = pos;
    }

    // Update is called once per frame
    void Update()
    {   
        if(rot) {
            yaw += velX * Input.GetAxis("Mouse X");
            pitch -= velY * Input.GetAxis("Mouse Y");
            yaw = Mathf.Clamp(yaw, originYaw - 10f, originYaw + 10f);
            pitch = Mathf.Clamp(pitch, originPitch - 10f, originPitch + 10f);
            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }
        else {
            pos[0] += velX * Input.GetAxis("Mouse X")/10;
            pos[1] += velY * Input.GetAxis("Mouse Y")/10;
            pos[0] = Mathf.Clamp(pos[0], originPos[0] - 2.0f, originPos[0] + 2.0f);
            pos[1] = Mathf.Clamp(pos[1], originPos[1] - 2.0f, originPos[1] + 2.0f);
            transform.position = pos;
        }
        
    }
}
