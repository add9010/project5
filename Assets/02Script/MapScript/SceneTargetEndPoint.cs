using UnityEngine;

public class SceneTargetEndPoint : MonoBehaviour
{
    public enum TargetSceneType { LoadScene, LoadWithCondition }

    public TargetSceneType targetType = TargetSceneType.LoadScene;
    public string sceneName;
    public string requiredStoryKey; // 조건부 이동용

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (targetType == TargetSceneType.LoadWithCondition)
            {
                if (StoryManager.Instance.HasProgress(requiredStoryKey))
                {
                    GameManager.Instance.LoadSceneWithFade(sceneName);
                }
                else
                {
                    Debug.Log("조건 미달: 이동 불가");
                }
            }
            else
            {
                GameManager.Instance.LoadSceneWithFade(sceneName);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
