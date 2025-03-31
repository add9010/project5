using UnityEngine;

public static class DialogueLoader
{
    public static DialogueDataSet LoadDialogFromJSON(string fileName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Dialogues/" + fileName);
        if (jsonFile == null)
        {
            Debug.LogError($"Dialogues/{fileName}.json ������ ã�� �� �����ϴ�!");
            return null;
        }

        return JsonUtility.FromJson<DialogueDataSet>(jsonFile.text);
    }
}
