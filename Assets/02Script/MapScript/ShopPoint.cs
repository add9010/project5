// ShopPoint.cs
using UnityEngine;

public class ShopPoint : MonoBehaviour
{
    public int shopIndex;
    public bool isExit = false; // 출구인가? (false면 입구)

    private bool playerInRange = false;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (isExit)
            {
                // ── “출구”일 때: 이전 맵으로 돌아가기
                MapManager.Instance.ReturnToPreviousMap();
            }
            else
            {
                // ── “입구”일 때: 현재 맵 상태(인덱스+플레이어 위치) 저장 후 상점으로 이동
                MapManager.Instance.SaveMapState();
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
