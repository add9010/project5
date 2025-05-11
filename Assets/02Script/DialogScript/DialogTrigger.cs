using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [Header("설정")]
    public string npcName; // 예: Oxton, Neroban, Atti, Ideer
    public DialogSystem dialogSystem;

    private DialogueDataSet dataSet;
    private bool isPlayerInRange = false;

    private void Start()
    {
        string fileToLoad = "Default";

        int stage = GameManager.Instance.gameData.currentStage;
        var data = GameManager.Instance.gameData;

        switch (npcName)
        {
            case "Oxton":
                fileToLoad = (stage >= 1) ? "Oxton_AfterStage1" : "Oxton_BeforeQuest";
                break;

            case "Neroban":
                fileToLoad = (stage >= 1) ? "Neroban_AfterStage1" : "Neroban_BeforeQuest";
                break;

            case "Atti":
                fileToLoad = (stage >= 1) ? "Atti_AfterStage1" : "Atti_BeforeQuest";
                break;

            case "Ideer":
                if (!data.IsQuestComplete("Quest001"))
                    fileToLoad = "Ideer_QuestOffer";
                else
                    fileToLoad = "Ideer_AfterStage1";
                break;

            default:
                Debug.LogWarning($"DialogTrigger: 알 수 없는 NPC 이름 '{npcName}'");
                break;
        }

        dataSet = DialogueLoader.LoadDialogFromJSON(fileToLoad);
        if (dataSet == null)
        {
            Debug.LogError($"Dialogues/{fileToLoad}.json 파일을 찾을 수 없습니다!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    public void TryStartDialogue()
    {
        if (isPlayerInRange && dataSet != null)
        {
            dialogSystem.StartDialogue(dataSet.dialogues);
            PlayerManager.Instance.isAction = true;
        }
    }
}
