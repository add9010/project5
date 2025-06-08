// ShopPoint.cs
using UnityEngine;

public class ShopPoint : MonoBehaviour
{
    [Header("상점 설정")]
    public int shopIndex;             // 몇 번 상점(ShopMaps index)
    public bool isExit = false;       // 출구 트리거인가?
    public Transform doorPoint;       // 문 위치를 드래그해서 할당

    private bool playerInRange = false;

    private void Update()
    {
        if (!playerInRange || Input.GetKeyDown(KeyCode.F) == false) return;

        if (isExit)
        {
            // 출구: doorPoint 위치로 복귀
            if (doorPoint != null)
                MapManager.Instance.ReturnToPreviousMapAt(doorPoint.position);
            else
                MapManager.Instance.ReturnToPreviousMap();
        }
        else
        {
            // 입구: 문 위치를 저장 후 상점 진입
            if (doorPoint != null)
                MapManager.Instance.SaveMapState(doorPoint.position);
            else
                MapManager.Instance.SaveMapState();

            MapManager.Instance.GoToShopMap(shopIndex);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }
}
