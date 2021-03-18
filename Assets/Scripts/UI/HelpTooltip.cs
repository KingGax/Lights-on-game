using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpTooltip : MonoBehaviour
{
    public GameObject controlHelpObject;
    bool visible = true;
    // Start is called before the first frame update
    void Start()
    {
        //controlHelpObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToggleVisibility() {
        controlHelpObject.SetActive(!visible);
        visible = !visible;
    }
}
