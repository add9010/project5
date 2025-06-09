using UnityEngine;

public class HealthUpPickup : MonoBehaviour
{
    [Tooltip("증가시킬 최대 체력 양")]
    [SerializeField] private float buffAmount = 50f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어 태그 확인
        if (!other.CompareTag("Player")) return;

        // 1) 최대 체력 즉시 증가
        PlayerManager.Instance.data.maxHealth += buffAmount;

        // 2) UI 갱신 (현재 체력은 변하지 않으므로 currentHealth만 넘겨줌)
        PlayerManager.Instance.UpdateHpUI(
            PlayerManager.Instance.playerHealth.currentHealth
        );

        // 3) 세이브
        GameManager.Instance.SaveGame();

        // 4) 픽업 오브젝트 제거
        Destroy(gameObject);
    }
}
