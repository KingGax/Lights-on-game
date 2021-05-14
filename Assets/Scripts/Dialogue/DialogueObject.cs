using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class DialogueInfo
{   
    [TextArea]
    public string dialogue;
    public int playerIndex;
}

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject{
    [SerializeField]  public DialogueInfo[] dialogue;

    public DialogueInfo[] Dialogue => dialogue;

}
