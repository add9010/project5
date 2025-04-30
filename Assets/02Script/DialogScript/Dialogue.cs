using UnityEngine;

[System.Serializable]
public class DialogueOption
{
    public string optionText;
    public int nextDialogueIndex;
}

[System.Serializable]
public class Dialogue
{
    public string speakerName;
    public string speakerPortraitPath;
    [System.NonSerialized]
    public Sprite speakerPortrait; // Resources에서 로딩할 경우 string으로 교체해도 됩니다
    public string[] sentences;
    public DialogueOption[] options;
}

[System.Serializable]
public class DialogueDataSet
{
    public Dialogue[] dialogues;
}
