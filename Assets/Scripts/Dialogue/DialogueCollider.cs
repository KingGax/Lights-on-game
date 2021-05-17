using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueCollider : MonoBehaviour
{
    [SerializeField]
    private int targetCollisions;
    private int numberOfCollisions = 0;

    private bool activated = false;

    [SerializeField]
    private DialogueUI dialogueUI;

    [SerializeField]
    private DialogueObject dialogueObject;

    [SerializeField]
    private AfterDialogue afterDialogue;

    void Start(){
    }

    void Update(){

    }

    private void OnTriggerEnter(){
        numberOfCollisions++;
        if(numberOfCollisions == targetCollisions && !activated){
            activated = true;
            if(afterDialogue == null) dialogueUI.ShowDialogue(dialogueObject);
            else dialogueUI.ShowDialogue(dialogueObject, afterDialogue);
        }
    }

    private void OnTriggerExit(){
        numberOfCollisions--;
    }
}
