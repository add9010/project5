using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DialogSystem : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject panel;                         // 전체 대화 패널
    public TextMeshProUGUI speakerNameText;          // 말하는 캐릭터 이름
    public TextMeshProUGUI sentenceText;             // 대사 본문
    public Image portraitImage;                      // 캐릭터 초상화
    public GameObject optionsPanel;                  // 선택지 버튼들을 담는 패널
    public Button optionButtonPrefab;                // 선택지 버튼 프리팹

    private Queue<string> sentences = new Queue<string>();  // 현재 대화에서 출력할 문장들
    private Dialogue[] dialogues;                            // 현재 대화 전체 구조
    private int currentDialogueIndex = 0;                    // 현재 진행 중인 대화 인덱스
    private bool isTyping = false;                           // 텍스트 타이핑 중 여부
    private List<Button> currentOptionButtons = new List<Button>();// 선택지 버튼들을 임시로 저장하는 리스트

    // 대화 시작 진입점
    public void StartDialogue(Dialogue[] loadedDialogues)
    {
        dialogues = loadedDialogues;
        currentDialogueIndex = 0;
        panel.SetActive(true);
        ShowCurrentDialogue();
    }

    // 현재 인덱스에 해당하는 대사 내용을 UI에 표시
    void ShowCurrentDialogue()
    {
        if (currentDialogueIndex >= dialogues.Length)
        {
            EndDialogue();
            return;
        }

        Dialogue dialogue = dialogues[currentDialogueIndex];

        // UI 요소 업데이트
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

    // 다음 문장 출력 or 선택지 표시
    public void DisplayNextSentence()
    {
        if (isTyping) return;

        if (sentences.Count == 0)
        {
            // 선택지가 있는 경우 표시
            if (dialogues[currentDialogueIndex].options != null &&
                dialogues[currentDialogueIndex].options.Length > 0)
            {
                ShowOptions(dialogues[currentDialogueIndex].options);
            }
            else
            {
                // 다음 대사로 넘어감
                currentDialogueIndex++;
                ShowCurrentDialogue();
            }
            return;
        }

        // 다음 문장 하나 꺼내서 출력
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    // 문자를 한 글자씩 출력하는 효과
    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        sentenceText.text = "";

        foreach (char letter in sentence)
        {
            sentenceText.text += letter;
            yield return new WaitForSeconds(0.02f); // 출력 속도
        }

        isTyping = false;

        // 문장 끝난 뒤 선택지가 있다면 바로 표시
        if (sentences.Count == 0 &&
            dialogues[currentDialogueIndex].options != null &&
            dialogues[currentDialogueIndex].options.Length > 0)
        {
            ShowOptions(dialogues[currentDialogueIndex].options);
        }
    }

    // 선택지 버튼을 생성하고 이벤트 연결
    void ShowOptions(DialogueOption[] options)
    {
        optionsPanel.SetActive(true);

        // 기존 버튼 삭제
        foreach (Transform child in optionsPanel.transform)
            Destroy(child.gameObject);

        // 버튼 리스트 초기화
        currentOptionButtons.Clear();

        foreach (DialogueOption option in options)
        {
            // 버튼 생성
            Button btn = Instantiate(optionButtonPrefab, optionsPanel.transform);

            if (btn == null)
            {
                Debug.LogError("버튼 생성 실패: 프리팹이 null입니다.");
                continue;
            }

            // 버튼 텍스트 설정
            var tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
                tmp.text = option.optionText;

            // 클릭 시 다음 대화 인덱스로 이동
            int nextIndex = option.nextDialogueIndex;
            btn.onClick.AddListener(() =>
            {
                Debug.Log($"선택지 '{option.optionText}' 선택됨 → 인덱스 {nextIndex}");
                currentDialogueIndex = nextIndex;
                ShowCurrentDialogue();
            });

            //숫자키 입력을 위한 리스트에 추가
            currentOptionButtons.Add(btn);
        }
    }

    // Space 키로 대사 넘기기 (선택지가 떠있지 않을 때만)
    void Update()
    {
        // 대사 넘기기
        if (Input.GetKeyDown(KeyCode.Space) && panel.activeSelf && !optionsPanel.activeSelf)
        {
            DisplayNextSentence();
        }

        // 숫자키로 선택지 고르기
        if (panel.activeSelf && optionsPanel.activeSelf)
        {
            for (int i = 0; i < currentOptionButtons.Count && i < 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    Debug.Log($"숫자키 {i + 1} 입력됨");
                    currentOptionButtons[i].onClick.Invoke();
                }
            }
        }
    }



    // 대화 종료 처리
    void EndDialogue()
    {
        panel.SetActive(false);
        PlayerManager.Instance.isAction = false; // 플레이어 행동 재개
    }
}
