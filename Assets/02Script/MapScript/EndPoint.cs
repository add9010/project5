using UnityEngine;

public class EndPoint : MonoBehaviour
{
    private bool playerInRange = false;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (IsRiverStage())
            {
                if (MapManager.Instance != null)
                {
                    MapManager.Instance.ClearRiverStage();
                }
            }

            MapManager.Instance.GoToNextMap();
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

    private bool IsRiverStage()
    {
        string currentMapName = MapManager.Instance.GetCurrentMapName();
        return currentMapName.Contains("RiverStage");
    }
}
