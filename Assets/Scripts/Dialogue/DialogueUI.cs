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

    [SerializeField]
    private TMP_Text keyPressPrompt;

    [SerializeField]
    private GlobalValues globalValues;
    void Start(){
        typeWriterEffect = GetComponent<TypeWriterEffect>();
        CloseDialogueBox();
        //ShowDialogue(testDialogue);
    }

    public void ShowDialogue(DialogueObject dialogueObject){
        DisableLocalPlayerMovement();
        OpenDialogueBox();
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject){
        
        foreach(string dialogue in dialogueObject.Dialogue){
            yield return typeWriterEffect.Run(dialogue, textLabel);
            ShowKeyPrompt();
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            HideKeyPrompt();
        }
        CloseDialogueBox();
        EnableLocalPlayerMovement();
    }

    private void CloseDialogueBox(){
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;
    }

    private void OpenDialogueBox(){
        dialogueBox.SetActive(true);
    }

    private void DisableLocalPlayerMovement(){
        GlobalValues.Instance.localPlayerInstance.GetComponent<PlayerInputScript>().DisableInput();
    }

    private void EnableLocalPlayerMovement(){
        GlobalValues.Instance.localPlayerInstance.GetComponent<PlayerInputScript>().EnableInput();
    }

    private void ShowKeyPrompt(){
        keyPressPrompt.gameObject.SetActive(true);
    }

    private void HideKeyPrompt(){
        keyPressPrompt.gameObject.SetActive(false);
    }

}
