using UnityEngine;

public class EndPoint : MonoBehaviour
{
    private bool playerInRange = false;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.UpArrow))
        {
            MapManager.Instance.GoToNextMap();

            if (IsRiverStage4())
            {
                MapManager.Instance.ClearRiverStage();

                // 이 시점에 스토리 키 등록
                StoryManager.Instance.SetProgress("RiverStage4Clear");
                GameManager.Instance.gameData.clearedStoryKeys.Add("RiverStage4Clear");
                GameManager.Instance.SaveGame();

                GameObject villageSavePoint = GameObject.Find("Village_SavePoint");
                if (villageSavePoint != null)
                {
                    villageSavePoint.SetActive(true);
                }
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private bool IsRiverStage4()
    {
        string currentMapName = MapManager.Instance.GetCurrentMapName();
        return currentMapName.Contains("RiverStage_4");
    }

}
