using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicSpriteController : MonoBehaviour
{
    Transform parentTransform;
    // Start is called before the first frame update
    void Start()
    {
        parentTransform = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, -parentTransform.rotation.y - 148.5f, 0);
    }
}
