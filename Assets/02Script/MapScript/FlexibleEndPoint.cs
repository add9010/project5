using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FlexibleEndPoint : MonoBehaviour
{
    public enum TargetType { NextInOrder, FixedMapIndex, LoadScene, LoadSceneWithCondition }

    public TargetType targetType = TargetType.NextInOrder;
    public string requiredStoryKey;

    
    [Header("Only for FixedMapIndex")]
    public int targetMapIndex = -1;

    [Header("Only for LoadScene")]
    public string sceneName;

    private bool playerInRange = false;

    public GameObject warningPanel;
    public Text warningText;
    public float warningDuration = 3f;
    private void Start()
    {
        // 시작할 때 숨겨두기
        if (warningPanel != null)
            warningPanel.SetActive(false);
    }
    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (!StoryManager.Instance.HasProgress("Quest001Accepted"))
            {
                ShowWarning("의뢰를 수락해야 이동할 수 있습니다.");
                return;
            }

            //RiverStage_4에서만 동작할 로직 추가
            if (IsRiverStage4())
            {
                MapManager.Instance.ClearRiverStage();

                if (!StoryManager.Instance.HasProgress("RiverStage4Clear"))
                {
                    StoryManager.Instance.SetProgress("RiverStage4Clear");
                    GameManager.Instance.gameData.clearedStoryKeys.Add("RiverStage4Clear");
                    GameManager.Instance.gameData.currentStage++;
                }
                if (!StoryManager.Instance.HasProgress("Quest001Completed"))
                {
                    StoryManager.Instance.SetProgress("Quest001Completed");
                    Debug.Log("Ideer 퀘스트 완료 처리됨.");
                }
                GameManager.Instance.SaveGame();

                GameObject villageSavePoint = GameObject.Find("Village_SavePoint");
                if (villageSavePoint != null)
                {
                    villageSavePoint.SetActive(true);
                }
            }

            //공통 이동 처리
            switch (targetType)
            {
                case TargetType.NextInOrder:
                    MapManager.Instance.GoToNextMap();
                    break;

                case TargetType.FixedMapIndex:
                    MapManager.Instance.ForceMoveToMap(targetMapIndex);
                    break;

                case TargetType.LoadScene:
                    SceneManager.LoadScene(sceneName);
                    break;

                case TargetType.LoadSceneWithCondition:
                    if (StoryManager.Instance.HasProgress(requiredStoryKey))
                        GameManager.Instance.LoadSceneWithFade(sceneName);
                    else
                        Debug.Log("조건 미달: 이동 불가");
                    break;
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
            playerInRange = false;
    }
    private void ShowWarning(string message)
    {
        if (warningPanel == null || warningText == null) return;

        warningText.text = message;
        warningPanel.SetActive(true);

        // 혹시 남아있는 Invoke 있다면 취소
        CancelInvoke(nameof(HideWarning));
        // warningDuration 초 뒤에 HideWarning() 호출
        Invoke(nameof(HideWarning), warningDuration);
    }
    private void HideWarning()
    {
        if (warningPanel != null)
            warningPanel.SetActive(false);
    }
    private bool IsRiverStage4()
    {
        return MapManager.Instance != null &&
               MapManager.Instance.GetCurrentMapName().Contains("RiverStage_4");
    }
}
