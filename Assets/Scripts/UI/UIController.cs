using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //DontDestroyOnLoad(this.gameObject);
        GlobalValues.Instance.UIElements = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}