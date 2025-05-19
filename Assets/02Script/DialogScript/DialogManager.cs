using UnityEngine;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [SerializeField] private GameObject talkPanel;
    [SerializeField] private TextMeshProUGUI talkText;

    private GameObject scanObject;
    public bool isAction { get; private set; } = false;

    private void Awake()
    {
        // ② 싱글톤 + 중복 방지
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // ③ 만약 Inspector에 할당이 누락됐다면 자동 탐색
        if (talkPanel == null)
            talkPanel = GameObject.Find("ManagersContainer/Canvas/DialogPanel");
        if (talkText == null && talkPanel != null)
            talkText = talkPanel.GetComponentInChildren<TextMeshProUGUI>();

        // 시작 시에는 항상 숨겨 두기
        if (talkPanel != null)
            talkPanel.SetActive(false);
    }
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
