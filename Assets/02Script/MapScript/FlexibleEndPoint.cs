using UnityEngine;
using UnityEngine.SceneManagement;

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

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
           //RiverStage_4에서만 동작할 로직 추가
            if (IsRiverStage4())
            {
                MapManager.Instance.ClearRiverStage();

                StoryManager.Instance.SetProgress("RiverStage4Clear");
                GameManager.Instance.gameData.clearedStoryKeys.Add("RiverStage4Clear");
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

    private bool IsRiverStage4()
    {
        return MapManager.Instance != null &&
               MapManager.Instance.GetCurrentMapName().Contains("RiverStage_4");
    }
}
