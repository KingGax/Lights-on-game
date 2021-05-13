using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueObj")]

[System.Serializable]
public class DialogueInfo
{   
    [TextArea]
    public string dialogue;
    public int playerIndex;
}
public class DialogueObject : ScriptableObject{
    [SerializeField]  public DialogueInfo[] dialogue;

    public DialogueInfo[] Dialogue => dialogue;

}
