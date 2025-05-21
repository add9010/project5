// StageSelectUIManager.cs - 업데이트 버전
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StageSelectUIManager : MonoBehaviour
{
    [System.Serializable]
    public class StageInfo
    {
        public string stageName;           // 이동할 씬 이름
        public GameObject panel;           // 선택 패널 (배경 + 버튼 포함 가능)
        public Button enterButton;         // 개별 스테이지 진입 버튼
        public string requiredStoryKey;    // 해금 조건 키

        public bool isUnlocked => string.IsNullOrEmpty(requiredStoryKey) || StoryManager.Instance.HasProgress(requiredStoryKey);
    }

    public StageInfo[] stages;
    public int currentIndex = 0;

    [Header("UI Elements")]
    public TextMeshProUGUI stageTitle;
    public Button returnToVillageButton;

    private void Start()
    {
        foreach (var stage in stages)
        {
            stage.enterButton.onClick.AddListener(() => TryEnterStage(stage));
        }
        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveCursor(-1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveCursor(1);
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            TryEnterStage(stages[currentIndex]);
        }
    }

    private void MoveCursor(int direction)
    {
        currentIndex = (currentIndex + direction + stages.Length) % stages.Length;
        UpdateUI();
    }

    private void UpdateUI()
    {
        for (int i = 0; i < stages.Length; i++)
        {
            stages[i].panel.SetActive(i == currentIndex);
            stages[i].enterButton.interactable = stages[i].isUnlocked;
        }

        if (stageTitle != null)
            stageTitle.text = stages[currentIndex].stageName;
    }

    public void TryEnterStage(StageInfo stage)
    {
        if (stage.isUnlocked)
        {
            GameManager.Instance.LoadSceneWithFade(stage.stageName);
        }
        else
        {
            Debug.Log("해금되지 않은 스테이지입니다");
        }
    }
    public void OnClick_River()
    {
        foreach (var stage in stages)
        {
            if (stage.stageName == "RiverStage")
            {
                TryEnterStage(stage);
                return;
            }
        }
        Debug.LogWarning("RiverStage 항목이 없습니다.");
    }

    public void OnClick_Golem()
    {
        foreach (var stage in stages)
        {
            if (stage.stageName == "GolemStage")
            {
                TryEnterStage(stage);
                return;
            }
        }
        Debug.LogWarning("GolemStage 항목이 없습니다.");
    }

    public void OnClick_ReturnToVillage()
    {
        GameManager.Instance.LoadSceneWithFade("VillageStage");
    }
}
