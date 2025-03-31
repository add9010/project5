using UnityEngine;

public static class DialogueLoader
{
    public static DialogueDataSet LoadDialogFromJSON(string fileName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Dialogues/" + fileName);
        if (jsonFile == null)
        {
            Debug.LogError($"Dialogues/{fileName}.json 파일을 찾을 수 없습니다!");
            return null;
        }

        return JsonUtility.FromJson<DialogueDataSet>(jsonFile.text);
    }
}
