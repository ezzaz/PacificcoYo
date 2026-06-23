using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [TextArea(4, 6)]
    public string text;
    public string characterName;
    public Sprite characterImage;
    public bool isRightSpeaker;
    public AudioClip nextDialogueSound;
}