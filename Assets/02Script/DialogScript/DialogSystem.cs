using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogSystem : MonoBehaviour
{
    [Header("UI 속성")]
    public GameObject panel;                         // 전체 대화 패널
    public TextMeshProUGUI speakerNameText;          // 말하는 캐릭터 이름
    public TextMeshProUGUI sentenceText;             // 대사 본문
    public Image portraitImage;                      // 캐릭터 초상화
    public GameObject optionsPanel;                  // 선택지 버튼들을 담는 패널
    public Button optionButtonPrefab;                // 선택지 버튼 프리팹

    public GameObject logPanel;                      // 로그 창 패널
    public GameObject autoAdvanceIndicator;          // 우상단 자동 진행 표시 UI
    public GameObject expandDialogButton;            // 대화창 숨김/펼치기 상태 표시용 UI

    // 대화 데이터 관련 변수
    private Queue<string> sentences = new Queue<string>();  // 현재 대화에서 출력할 문장들
    private Dialogue[] dialogues;                           // 현재 대화 전체 구조
    private int currentDialogueIndex = 0;                   // 현재 진행 중인 대화 인덱스

    // 상태 플래그
    private bool isTyping = false;          // 텍스트 타이핑 중 여부
    private bool isAutoAdvance = false;     // 자동 진행 플래그
    private bool isSkipRequested = false;   // 스킵 요청 플래그 (타이핑 중 스킵 감지)

    // 선택지 버튼 리스트
    private List<Button> currentOptionButtons = new List<Button>();

    // 자동 진행 UI의 Image 컴포넌트를 저장할 변수 (autoAdvanceIndicator에 연결된 오브젝트에서 가져옴)
    private Image autoAdvanceImage;
    // 대화창 숨김/펼치기 상태를 표시할 Image (expandDialogButton에 연결)
    private Image hideDialogImage;

    void Awake()
    {
        // autoAdvanceIndicator에서 Image 컴포넌트 가져오기
        if (autoAdvanceIndicator != null)
        {
            autoAdvanceImage = autoAdvanceIndicator.GetComponent<Image>();
            if (autoAdvanceImage != null)
                autoAdvanceImage.color = Color.white;
        }
        // expandDialogButton에서 Image 컴포넌트 가져오기 (숨김 상태 표시용)
        if (expandDialogButton != null)
        {
            hideDialogImage = expandDialogButton.GetComponent<Image>();
            if (hideDialogImage != null)
                hideDialogImage.color = Color.white;
        }
    }

    // ------------------- Public Methods -------------------

    // 대화 시작 진입점
    public void StartDialogue(Dialogue[] loadedDialogues)
    {
        dialogues = loadedDialogues;
        currentDialogueIndex = 0;
        panel.SetActive(true);
        ShowCurrentDialogue();
    }

    // 펼치기 버튼 (여기서는 별도로 사용하지 않음)
    public void ExpandDialog()
    {
        panel.SetActive(true);
        // 상태 표시 UI의 색상을 보이는 상태로 변경
        if (hideDialogImage != null)
            hideDialogImage.color = Color.white;
    }

    // ------------------- Private Methods -------------------

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
        if (dialogue.speakerPortrait != null)
        {
            portraitImage.sprite = dialogue.speakerPortrait;
        }
        sentences.Clear();
        foreach (var sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        sentenceText.text = "";
        optionsPanel.SetActive(false);
        DisplayNextSentence();
    }

    // 다음 문장 출력 또는 선택지 표시
    public void DisplayNextSentence()
    {
        if (isTyping)
            return;

        if (sentences.Count == 0)
        {
            // 선택지가 있는 경우 표시
            if (dialogues[currentDialogueIndex].options != null &&
                dialogues[currentDialogueIndex].options.Length > 0)
            {
                // 옵션이 있으면 자동 진행 모드를 중단하고, 자동 진행 UI 색상을 원래대로 복원
                isAutoAdvance = false;
                if (autoAdvanceImage != null)
                {
                    autoAdvanceImage.color = Color.white;
                }
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

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    // 문자를 한 글자씩 출력하는 효과 (스킵 기능 및 자동 진행 포함)
    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        sentenceText.text = "";
        foreach (char letter in sentence)
        {
            if (isSkipRequested)
            {
                sentenceText.text = sentence;
                break;
            }
            sentenceText.text += letter;
            yield return new WaitForSeconds(0.08f);
        }
        isTyping = false;
        isSkipRequested = false;

        // 자동 진행 모드가 활성화되어 있고 선택지가 없으면 자동으로 다음 문장 호출
        if (sentences.Count == 0 &&
        dialogues[currentDialogueIndex].options != null &&
        dialogues[currentDialogueIndex].options.Length > 0)
        {
            // 선택지가 있으면 자동 진행 모드를 중단하고, UI 색상을 원래대로 복원한 후 선택지 표시
            isAutoAdvance = false;
            if (autoAdvanceImage != null)
            {
                autoAdvanceImage.color = Color.white;
            }
            ShowOptions(dialogues[currentDialogueIndex].options);
        }
        else if (isAutoAdvance)
        {
            // 자동 진행 모드가 활성화되어 있으면, 딜레이 후 다음 문장 호출
            yield return new WaitForSeconds(1.0f);
            DisplayNextSentence();
        }
    }

    // 선택지 버튼 생성 및 이벤트 연결
    void ShowOptions(DialogueOption[] options)
    {
        optionsPanel.SetActive(true);
        foreach (Transform child in optionsPanel.transform)
            Destroy(child.gameObject);
        currentOptionButtons.Clear();
        foreach (DialogueOption option in options)
        {
            Button btn = Instantiate(optionButtonPrefab, optionsPanel.transform);
            if (btn == null)
            {
                Debug.LogError("버튼 생성 실패: 프리팹이 null입니다.");
                continue;
            }
            var tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
                tmp.text = option.optionText;
            int nextIndex = option.nextDialogueIndex;
            btn.onClick.AddListener(() =>
            {
                Debug.Log($"선택지 '{option.optionText}' 선택됨 → 인덱스 {nextIndex}");
                if (nextIndex == -1)
                {
                    EndDialogue();
                }
                else
                {
                    currentDialogueIndex = nextIndex;
                    ShowCurrentDialogue();
                }
            });
            currentOptionButtons.Add(btn);
        }
    }


    // 대화 종료 처리
    void EndDialogue()
    {
        panel.SetActive(false);
        PlayerManager.Instance.isAction = false;
    }

    // ------------------- Update Method -------------------

    void Update()
    {
        // Space 키: 선택지가 떠있지 않을 때 대화 진행
        if (Input.GetKeyDown(KeyCode.Space) && panel.activeSelf && !optionsPanel.activeSelf)
        {
            DisplayNextSentence();
        }

        // 숫자키로 선택지 고르기 (1~9)
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

        // 스킵 기능: '.' 키 (문장 타이핑 중)
        if (Input.GetKeyDown(KeyCode.Period))
        {
            if (isTyping)
            {
                isSkipRequested = true;
            }
            else
            {
                DisplayNextSentence();
            }
        }

        // 로그 패널 토글: ',' 키
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            logPanel.SetActive(!logPanel.activeSelf);
        }

        // 자동 진행 토글: M 키 → 색상 변경만 (항상 활성 상태)
        if (Input.GetKeyDown(KeyCode.M))
        {
            isAutoAdvance = !isAutoAdvance;
            if (autoAdvanceImage != null)
            {
                autoAdvanceImage.color = isAutoAdvance ? Color.green : Color.white;
            }
        }

        // 대화창 숨기기 토글: '/' 키 → 패널 활성 상태를 토글하고, 숨김 상태를 색상으로 표시
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            bool isVisible = panel.activeSelf;
            panel.SetActive(!isVisible);
        }
    }
}
