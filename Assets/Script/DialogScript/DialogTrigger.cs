using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public string jsonFileName;
    public DialogSystem dialogSystem;

    private DialogueDataSet dataSet;
    private bool isPlayerInRange = false;

    private void Start()
    {
        dataSet = DialogueLoader.LoadDialogFromJSON(jsonFileName);
        if (dataSet == null)
        {
            Debug.LogError($"Dialogues/{jsonFileName}.json 파일을 찾을 수 없습니다!");
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
