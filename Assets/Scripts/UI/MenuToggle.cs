using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuToggle : MonoBehaviour
{
    public GameObject MenuItem;
    bool visible = false;
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
    public bool ToggleVisibility() {
        MenuItem.SetActive(!visible);
        visible = !visible;
        return visible;
    }
}
