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

        DialogueDataSet dataSet = JsonUtility.FromJson<DialogueDataSet>(jsonFile.text);

        // 각 대화에 대해 스프라이트 로드
        foreach (Dialogue dialogue in dataSet.dialogues)
        {
            if (!string.IsNullOrEmpty(dialogue.speakerPortraitPath))
            {
                // Resources 폴더 내 적절한 경로를 지정 (예: "Portraits/GreenPortrait")
                dialogue.speakerPortrait = Resources.Load<Sprite>(dialogue.speakerPortraitPath);
                if (dialogue.speakerPortrait == null)
                    Debug.LogWarning($"스프라이트를 로드하지 못했습니다: {dialogue.speakerPortraitPath}");
            }
        }

        return dataSet;
    }
}
