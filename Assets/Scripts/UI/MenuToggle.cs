using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuToggle : MonoBehaviour
{
    public GameObject MenuItem;
    bool visible = true;
    // Start is called before the first frame update
    void Start()
    {
        GlobalValues.Instance.MenuItem = MenuItem;
        MenuItem.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToggleVisibility() {
        MenuItem.SetActive(!visible);
        visible = !visible;
    }
}
