using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueCollider : MonoBehaviour
{
    [SerializeField]
    private int targetCollisions;
    private int numberOfCollisions = 0;
    public int waitForEventIndex = -1;

    private bool activated = false;

    [SerializeField]
    private DialogueUI dialogueUI;

    [SerializeField]
    private DialogueObject dialogueObject;

    [SerializeField]
    private AfterDialogue afterDialogue;

    void Start(){
        Debug.Log("I AM A TRIGGER");
    }

    void Update(){

    }

    private void OnTriggerEnter(){
        numberOfCollisions++;
        //Debug.Log("number of collisions: " + numberOfCollisions);
       
        if(numberOfCollisions == targetCollisions && !activated){
            if (waitForEventIndex == -1 || GlobalValues.Instance.fm.levels[waitForEventIndex].complete){
                activated = true;
                if(afterDialogue == null) dialogueUI.ShowDialogue(dialogueObject);
                else dialogueUI.ShowDialogue(dialogueObject, afterDialogue);
            }
        }
    }

    private void OnTriggerExit(){
        numberOfCollisions--;
        //Debug.Log("number of collisions: " + numberOfCollisions);
    }
}
