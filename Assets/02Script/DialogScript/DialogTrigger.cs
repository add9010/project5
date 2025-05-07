using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public string jsonFileName;
    public DialogSystem dialogSystem;

    private DialogueDataSet dataSet;
    private bool isPlayerInRange = false;

    private void Start()
    {
        string fileToLoad = jsonFileName;

        if (GameManager.Instance.gameData.isQuestComplete[0])
        {
            fileToLoad = "NPC_QuestComplete";
        }
        else
        {
            fileToLoad = "NPC_BeforeQuest";
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
            // 예: 대화 가능 UI 프롬프트를 표시할 수 있음
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
