using UnityEngine;

public class ReturnPoint : MonoBehaviour
{
    [Header("되돌아갈 맵 인덱스 (Village)")]
    public int returnMapIndex = 0;         // Village가 maps 배열의 0번이라면 0

    [Header("되돌아갈 위치 (ToCrossRoad Transform)")]
    public Transform returnPosition;       // 인스펙터에서 Village ▶ ToCrossRoad Transform을 드래그

    bool playerInRange = false;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player")) playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player")) playerInRange = false;
    }

    private void Update()
    {
        if (!playerInRange || !Input.GetKeyDown(KeyCode.F)) return;

        // 1) 현재 맵(예: CrossRoad) 비활성화, Village 활성화
        MapManager.Instance.ForceMoveToMap(returnMapIndex);

        // 2) 플레이어를 ToCrossRoad 위치로 강제 Teleport
        if (returnPosition != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                player.transform.position = returnPosition.position;
        }
    }
}
