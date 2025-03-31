using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DialogSystem : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject panel;                         // ��ü ��ȭ �г�
    public TextMeshProUGUI speakerNameText;          // ���ϴ� ĳ���� �̸�
    public TextMeshProUGUI sentenceText;             // ��� ����
    public Image portraitImage;                      // ĳ���� �ʻ�ȭ
    public GameObject optionsPanel;                  // ������ ��ư���� ��� �г�
    public Button optionButtonPrefab;                // ������ ��ư ������

    private Queue<string> sentences = new Queue<string>();  // ���� ��ȭ���� ����� �����
    private Dialogue[] dialogues;                            // ���� ��ȭ ��ü ����
    private int currentDialogueIndex = 0;                    // ���� ���� ���� ��ȭ �ε���
    private bool isTyping = false;                           // �ؽ�Ʈ Ÿ���� �� ����
    private List<Button> currentOptionButtons = new List<Button>();// ������ ��ư���� �ӽ÷� �����ϴ� ����Ʈ

    // ��ȭ ���� ������
    public void StartDialogue(Dialogue[] loadedDialogues)
    {
        dialogues = loadedDialogues;
        currentDialogueIndex = 0;
        panel.SetActive(true);
        ShowCurrentDialogue();
    }

    // ���� �ε����� �ش��ϴ� ��� ������ UI�� ǥ��
    void ShowCurrentDialogue()
    {
        if (currentDialogueIndex >= dialogues.Length)
        {
            EndDialogue();
            return;
        }

        Dialogue dialogue = dialogues[currentDialogueIndex];

        // UI ��� ������Ʈ
        speakerNameText.text = dialogue.speakerName;
        portraitImage.sprite = dialogue.speakerPortrait;
        sentences.Clear();

        foreach (var sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        sentenceText.text = "";
        optionsPanel.SetActive(false);
        DisplayNextSentence();
    }

    // ���� ���� ��� or ������ ǥ��
    public void DisplayNextSentence()
    {
        if (isTyping) return;

        if (sentences.Count == 0)
        {
            // �������� �ִ� ��� ǥ��
            if (dialogues[currentDialogueIndex].options != null &&
                dialogues[currentDialogueIndex].options.Length > 0)
            {
                ShowOptions(dialogues[currentDialogueIndex].options);
            }
            else
            {
                // ���� ���� �Ѿ
                currentDialogueIndex++;
                ShowCurrentDialogue();
            }
            return;
        }

        // ���� ���� �ϳ� ������ ���
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    // ���ڸ� �� ���ھ� ����ϴ� ȿ��
    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        sentenceText.text = "";

        foreach (char letter in sentence)
        {
            sentenceText.text += letter;
            yield return new WaitForSeconds(0.02f); // ��� �ӵ�
        }

        isTyping = false;

        // ���� ���� �� �������� �ִٸ� �ٷ� ǥ��
        if (sentences.Count == 0 &&
            dialogues[currentDialogueIndex].options != null &&
            dialogues[currentDialogueIndex].options.Length > 0)
        {
            ShowOptions(dialogues[currentDialogueIndex].options);
        }
    }

    // ������ ��ư�� �����ϰ� �̺�Ʈ ����
    void ShowOptions(DialogueOption[] options)
    {
        optionsPanel.SetActive(true);

        // ���� ��ư ����
        foreach (Transform child in optionsPanel.transform)
            Destroy(child.gameObject);

        // ��ư ����Ʈ �ʱ�ȭ
        currentOptionButtons.Clear();

        foreach (DialogueOption option in options)
        {
            // ��ư ����
            Button btn = Instantiate(optionButtonPrefab, optionsPanel.transform);

            if (btn == null)
            {
                Debug.LogError("��ư ���� ����: �������� null�Դϴ�.");
                continue;
            }

            // ��ư �ؽ�Ʈ ����
            var tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
                tmp.text = option.optionText;

            // Ŭ�� �� ���� ��ȭ �ε����� �̵�
            int nextIndex = option.nextDialogueIndex;
            btn.onClick.AddListener(() =>
            {
                Debug.Log($"������ '{option.optionText}' ���õ� �� �ε��� {nextIndex}");
                currentDialogueIndex = nextIndex;
                ShowCurrentDialogue();
            });

            //����Ű �Է��� ���� ����Ʈ�� �߰�
            currentOptionButtons.Add(btn);
        }
    }

    // Space Ű�� ��� �ѱ�� (�������� ������ ���� ����)
    void Update()
    {
        // ��� �ѱ��
        if (Input.GetKeyDown(KeyCode.Space) && panel.activeSelf && !optionsPanel.activeSelf)
        {
            DisplayNextSentence();
        }

        // ����Ű�� ������ ����
        if (panel.activeSelf && optionsPanel.activeSelf)
        {
            for (int i = 0; i < currentOptionButtons.Count && i < 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    Debug.Log($"����Ű {i + 1} �Էµ�");
                    currentOptionButtons[i].onClick.Invoke();
                }
            }
        }
    }



    // ��ȭ ���� ó��
    void EndDialogue()
    {
        panel.SetActive(false);
        PlayerManager.Instance.isAction = false; // �÷��̾� �ൿ �簳
    }
}
