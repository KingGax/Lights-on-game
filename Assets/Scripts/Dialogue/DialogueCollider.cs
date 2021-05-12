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

    private void OnTriggerEnter(){
        numberOfCollisions++;
        if(numberOfCollisions == targetCollisions && !activated){
            activated = true;
            dialogueUI.ShowDialogue(dialogueObject);
        }
    }

    private void OnTriggerExit(){
        numberOfCollisions--;
    }
}
