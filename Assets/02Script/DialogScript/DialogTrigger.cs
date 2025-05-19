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
        string fileToLoad = null;

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

            case "Tamyu":
                fileToLoad = (stage >= 1) ? "Tamyu_AfterStage1" : "Tamyu_BeforeQuest";
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

        if (!string.IsNullOrEmpty(fileToLoad))
        {
            dataSet = DialogueLoader.LoadDialogFromJSON(fileToLoad);
            if (dataSet == null)
            {
                Debug.LogError($"Dialogues/{fileToLoad}.json 파일을 찾을 수 없습니다!");
            }
        }
        else
        {
            Debug.LogError($"DialogTrigger: fileToLoad이 null입니다. NPC 이름 '{npcName}' 확인 필요.");
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
        // 범위 밖이거나 데이터 없으면 바로 리턴
        if (!isPlayerInRange || dataSet == null)
            return;

        // dialogSystem이 Inspector에 할당되지 않았으면 씬에서 찾아보기
        if (dialogSystem == null)
        {
            dialogSystem = Object.FindAnyObjectByType<DialogSystem>();
            if (dialogSystem == null)
            {
                Debug.LogError("DialogTrigger: DialogSystem 인스턴스를 찾을 수 없습니다!");
                return;
            }
        }

        // 이제 안전하게 대화 시작
        dialogSystem.StartDialogue(dataSet.dialogues);

        // PlayerManager.Instance에도 방어 코드
        if (PlayerManager.Instance != null)
            PlayerManager.Instance.isAction = true;
        else
            Debug.LogWarning("DialogTrigger: PlayerManager.Instance가 null입니다.");
    }
}
