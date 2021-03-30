using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavArrowController : MonoBehaviour
{
    Transform target;
    Transform origin;
    bool isEnabled;
    bool playerFound = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateTarget(Transform newTarget){
        target = newTarget;
    }

    float AngleDir(Vector3 targetVec) {
        //thank you https://forum.unity.com/threads/how-to-get-a-360-degree-vector3-angle.42145/
        Vector3 perp = Vector3.Cross(Vector3.forward, targetVec);
        float dir = Vector3.Dot(perp, Vector3.up);
        if (dir > 0.0) {
            return 1.0f;
        } else if (dir < 0.0) {
            return -1.0f;
        } else {
            return 0.0f;
        }
    }

    public void SetEnabled(bool state){
        isEnabled = state;
        GetComponent<Image>().enabled = state;
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnabled){
            if (!playerFound) {
                if (GlobalValues.Instance.localPlayerInstance != null) {
                    playerFound = true;
                    origin = GlobalValues.Instance.localPlayerInstance.transform;
                }
            }
            else if (target != null && origin != null){
                //need to rotate z based upon player's rotation difference 
                float rotation = Vector3.Angle(Vector3.forward, target.position - origin.position) * (-1f * AngleDir(target.position - origin.position)) + 45f;
                transform.rotation = Quaternion.Euler(0, 0, rotation);
            }
        }
    }
}
