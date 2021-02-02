using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightObject : MonoBehaviour
{
    // Start is called before the first frame update

    public Color colour { get; set; }
    Light playerLantern;

    void Start()
    {
        playerLantern = GetComponent<Light>();
        colour = playerLantern.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
