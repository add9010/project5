using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public string jsonFileName; 
    public DialogSystem dialogSystem;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var dataSet = DialogueLoader.LoadDialogFromJSON(jsonFileName);
            if (dataSet != null)
            {
                dialogSystem.StartDialogue(dataSet.dialogues);
                PlayerManager.Instance.isAction = true;
            }
        }
    }
}
