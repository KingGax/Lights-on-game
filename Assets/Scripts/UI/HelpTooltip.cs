using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpTooltip : MonoBehaviour
{
    public GameObject container;
    bool visible = false;
    // Start is called before the first frame update
    void Start()
    {
        container.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToggleVisibility() {
        container.SetActive(!visible);
        visible = !visible;
    }
}
