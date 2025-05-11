using UnityEngine;

public class ShopPoint : MonoBehaviour
{
    public int shopIndex;
    public bool isExit = false; // 출구냐? (false면 입구)

    private bool playerInRange = false;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (isExit)
            {
                // 출구일 때: 원래 맵으로 복귀
                MapManager.Instance.GoToNextMap();
            }
            else
            {
                // 입구일 때: 상점 입장
                MapManager.Instance.GoToShopMap(shopIndex);
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
}
