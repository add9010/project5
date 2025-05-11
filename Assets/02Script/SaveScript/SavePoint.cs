using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public string requiredStoryKey;  // 이 오브젝트를 활성화하기 위한 조건 키
  
    private bool isPlayerInRange = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = false;
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (IsAvailable())
            {
                GameManager.Instance.SaveGame();
            }
        }
    }

    private bool IsAvailable()
    {
        // requiredStoryKey가 비어있으면 항상 true
        return string.IsNullOrEmpty(requiredStoryKey) || StoryManager.Instance.HasProgress(requiredStoryKey);
    }

}
