using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {
    /*private void Awake() {
        //DontDestroyOnLoad(this.gameObject);
        GlobalValues.Instance.UIElements = gameObject;
    }*/
    [SerializeField]
    private GameObject LeaveButton;

    [SerializeField]
    private GameObject ControlsHelp;

    public void EnableLeaveButton(){
        LeaveButton.SetActive(true);
    }

    public void DisableLeaveButton(){
        LeaveButton.SetActive(false);
    }

    public void EnableControlsHelp(){
        ControlsHelp.SetActive(true);
    }

    public void DisableControlsHelp(){
        ControlsHelp.SetActive(false);
    }
}
