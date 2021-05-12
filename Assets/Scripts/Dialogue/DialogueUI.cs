using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{

    [SerializeField]
    private TMP_Text textLabel;

    private TypeWriterEffect typeWriterEffect;
    // Start is called before the first frame update

    [SerializeField]
    private DialogueObject testDialogue;

    [SerializeField]
    private GameObject dialogueBox;
    void Start(){
        typeWriterEffect = GetComponent<TypeWriterEffect>();
        CloseDialogueBox();
        ShowDialogue(testDialogue);
    }

    public void ShowDialogue(DialogueObject dialogueObject){
        OpenDialogueBox();
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject){
        
        foreach(string dialogue in dialogueObject.Dialogue){
            yield return typeWriterEffect.Run(dialogue, textLabel);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }
        CloseDialogueBox();
    }

    private void CloseDialogueBox(){
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;
    }

    private void OpenDialogueBox(){
        dialogueBox.SetActive(true);
    }

}
