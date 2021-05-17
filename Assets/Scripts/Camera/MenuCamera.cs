using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    public float velX = 2.0f;
    private float pitch;
    private float yaw;
    public float velY = 2.0f;
    private float originPitch, originYaw;
    // Start is called before the first frame update
    void Start()
    {
        pitch = transform.eulerAngles[0];
        yaw = transform.eulerAngles[1];
        originPitch = pitch;
        originYaw = yaw;
    }

    // Update is called once per frame
    void Update()
    {
        yaw += velX * Input.GetAxis("Mouse X") / yaw;
        pitch -= velY * Input.GetAxis("Mouse Y") / yaw;
        yaw = Mathf.Clamp(yaw, originYaw - 20f, originYaw + 20f);
        pitch = Mathf.Clamp(pitch, originPitch - 20f, originPitch + 20f);

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
}
