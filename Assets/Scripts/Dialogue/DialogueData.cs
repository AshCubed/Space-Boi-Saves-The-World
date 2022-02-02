using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public DialogueData()
    {

    }

    [TextArea(4, 4)]
    public List<string> conversationBlock;
}
