using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class DialogueUI : MonoBehaviour
{

    [SerializeField]
    private TMP_Text textLabel;

    private TypeWriterEffect typeWriterEffect;
    // Start is called before the first frame update

    [SerializeField]
    private DialogueObject testDialogue;

    [SerializeField]
    private TMP_Text nameText;

    [SerializeField]
    private GameObject dialogueBox;

    [SerializeField]
    private TMP_Text keyPressPrompt;

    [SerializeField]
    private GlobalValues globalValues;

    [SerializeField]
    private List<string> names;
    void Start(){
        typeWriterEffect = GetComponent<TypeWriterEffect>();
        CloseDialogueBox();
        foreach (Player p in PhotonNetwork.PlayerList)
		{
			names.Add(p.NickName.ToUpper());
		}

        //ShowDialogue(testDialogue);
    }

    private void GetNames() {
        names.Clear();
        foreach (Player p in PhotonNetwork.PlayerList) {
            names.Add(p.NickName.ToUpper());
        }
    }

    public void ShowDialogue(DialogueObject dialogueObject, AfterDialogue afterDialogue){
        GetNames();
        DisableLocalPlayerMovement();
        OpenDialogueBox();
        DisableUIElements();
        StartCoroutine(StepThroughDialogue(dialogueObject, afterDialogue));
    }

    public void ShowDialogue(DialogueObject dialogueObject){
        //Debug.LogError("showing dialogue");
        GetNames();
        DisableLocalPlayerMovement();
        OpenDialogueBox();
        DisableUIElements();
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject, AfterDialogue afterDialogue){
        
        foreach(DialogueInfo dialogueInfo in dialogueObject.Dialogue){
            nameText.text = names[dialogueInfo.playerIndex];
            yield return typeWriterEffect.Run(dialogueInfo.dialogue, textLabel);
            ShowKeyPrompt();
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            HideKeyPrompt();
            
        }
        CloseDialogueBox();
        EnableLocalPlayerMovement();
        EnableUIElements();
        afterDialogue.Effect();
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject){
        
        foreach(DialogueInfo dialogueInfo in dialogueObject.Dialogue){
            nameText.text = names[dialogueInfo.playerIndex];
            yield return typeWriterEffect.Run(dialogueInfo.dialogue, textLabel);
            ShowKeyPrompt();
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            HideKeyPrompt();
        }
        CloseDialogueBox();
        EnableLocalPlayerMovement();
        EnableUIElements();
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
        GlobalValues.Instance.localPlayerInstance.GetComponent<PlayerController>().AttackOne(false);
        GlobalValues.Instance.localPlayerInstance.GetComponent<PlayerController>().AttackAlt(false);
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

    private void EnableUIElements(){
        GlobalValues.Instance.UIElements.GetComponent<UIController>().EnableLeaveButton();
        GlobalValues.Instance.UIElements.GetComponent<UIController>().EnableControlsHelp();
    }

    private void DisableUIElements(){
        GlobalValues.Instance.UIElements.GetComponent<UIController>().DisableLeaveButton();
        GlobalValues.Instance.UIElements.GetComponent<UIController>().DisableControlsHelp();
    }

}
