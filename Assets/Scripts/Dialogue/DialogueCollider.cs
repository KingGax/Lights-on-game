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

    private void OnTriggerEnter(){
        numberOfCollisions++;
        Debug.Log("number of collisions: " + numberOfCollisions);
        if(numberOfCollisions == targetCollisions && !activated){
            activated = true;
            dialogueUI.ShowDialogue(dialogueObject, afterDialogue);
        }
    }

    private void OnTriggerExit(){
        numberOfCollisions--;
        Debug.Log("number of collisions: " + numberOfCollisions);
    }
}
