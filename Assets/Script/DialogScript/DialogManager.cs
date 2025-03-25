using UnityEngine;
using TMPro;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private GameObject talkPanel;
    [SerializeField] private TextMeshProUGUI talkText;

    private GameObject scanObject;
    public bool isAction { get; private set; } = false;

    public void Action(GameObject scanObj)
    {
        if (isAction)
        {
            EndDialog();
        }
        else
        {
            StartDialog(scanObj);
        }
    }

    private void StartDialog(GameObject scanObj)
    {
        isAction = true;
        PlayerManager.Instance.isAction = true;

        scanObject = scanObj;
        talkText.text = $"당신이 바라본 것은: <b>{scanObject.name}</b>";
        talkPanel.SetActive(true);
    }

    private void EndDialog()
    {
        isAction = false;
        PlayerManager.Instance.isAction = false;

        talkPanel.SetActive(false);
        talkText.text = "";
        scanObject = null;
    }
}
