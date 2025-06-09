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
    public string[] sentences;
    public DialogueOption[] options;
}

[System.Serializable]
public class DialogueDataSet
{
    public Dialogue[] dialogues;
}
