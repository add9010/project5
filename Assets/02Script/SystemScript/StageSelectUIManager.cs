using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StageSelectUIManager : MonoBehaviour
{
    [System.Serializable]
    public class StageInfo
    {
        public string stageName;    // 씬 이름
        public GameObject panel;      // 말풍선/박스 패널
        public Button enterButton;  // 패널 안 진입 버튼
        public RectTransform pinRect; // 대응하는 핀 위치 (UI Canvas 상의 RectTransform)

        public string requiredStoryKey;
        public bool isUnlocked =>
            string.IsNullOrEmpty(requiredStoryKey) ||
            StoryManager.Instance.HasProgress(requiredStoryKey);
    }

    [Header("Stages")]
    public StageInfo[] stages;       // [0]=River, [1]=Golem
    public int currentIndex = 0;

    [Header("Return")]
    public Button returnButton; 

    int TotalOptions => stages.Length + 1;

    void Awake()
    {
        // Panel에 CanvasGroup이 없으면 자동으로 붙여준다
        foreach (var s in stages)
        {
            if (s.panel.GetComponent<CanvasGroup>() == null)
                s.panel.AddComponent<CanvasGroup>();
        }
    }

    void Start()
    {
        // 버튼 연결
        for (int i = 0; i < stages.Length; i++)
        {
            int idx = i;
            stages[i].enterButton.onClick.AddListener(() => TryEnterStage(stages[idx]));
        }
        returnButton.onClick.AddListener(OnClick_Return);
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) MoveCursor(-1);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) MoveCursor(+1);
        else if (Input.GetKeyDown(KeyCode.F)
               || Input.GetKeyDown(KeyCode.Return))
        {
            if (currentIndex < stages.Length)
                TryEnterStage(stages[currentIndex]);
            else
                OnClick_Return();
        }
    }

    void MoveCursor(int dir)
    {
        currentIndex = (currentIndex + dir + TotalOptions) % TotalOptions;
        UpdateUI();
    }

    void UpdateUI()
    {
        // 1) River/Golem 패널만 α 조정
        for (int i = 0; i < stages.Length; i++)
        {
            var cg = stages[i].panel.GetComponent<CanvasGroup>();
            bool selected = (i == currentIndex);

            // 선택된 패널은 선명, 아니면 반투명
            cg.alpha = selected ? 1f : 0.4f;
            cg.interactable = selected;
            cg.blocksRaycasts = selected;

            // 버튼도 선택된 상태에서만 활성
            stages[i].enterButton.interactable = selected && stages[i].isUnlocked;

            // 2) 커넥터(Connector 이미지) 회전/위치 갱신
            var connector = stages[i].panel.transform
                                  .Find("Connector")
                                  .GetComponent<RectTransform>();

            // pin → panel 방향 벡터 (Canvas 좌표계)
            Vector2 pinPos = stages[i].pinRect.position;
            Vector2 panelPos = stages[i].panel.GetComponent<RectTransform>().position;
            Vector2 dir = pinPos - panelPos;

            // 로컬 회전
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            connector.rotation = Quaternion.Euler(0, 0, angle);
        }

        // 3) Return 버튼은 α 고정, 포커스만 이동
        if (currentIndex == stages.Length)
            returnButton.Select();
        else
            stages[currentIndex].enterButton.Select();
    }

    void TryEnterStage(StageInfo s)
    {
        if (s.isUnlocked)
            GameManager.Instance.LoadSceneWithFade(s.stageName);
        else
            Debug.Log("해금되지 않은 스테이지입니다.");
    }

    void OnClick_Return()
    {
        GameManager.Instance.LoadSceneWithFade("VillageStage");
    }
}
