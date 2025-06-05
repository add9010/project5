// TrapTile.cs
using UnityEngine;

public class TrapTile : MonoBehaviour
{
    [Header("트랩 기본 설정")]
    [Tooltip("트랩에 밟혔을 때 한 번 입힐 데미지")]
    public float damage = 10f;

    [Tooltip("계속 머무를 때 데미지를 입힐 간격(초 단위)")]
    public float damageInterval = 2.5f;

    // 플레이어가 트리거 안에 있는 상태에서 타이머 역할을 할 변수
    private float damageTimer = 0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어가 처음 트랩에 들어올 때 즉시 데미지 입히고 타이머 초기화
        if (collision.CompareTag("Player"))
        {
            PlayerManager player = collision.GetComponent<PlayerManager>();
            if (player != null)
            {
                player.playerHealth.TakeDamage(damage);
                damageTimer = 0f;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 플레이어가 트랩 안에 머무르는 동안 damageInterval마다 데미지 반복
        if (collision.CompareTag("Player"))
        {
            PlayerManager player = collision.GetComponent<PlayerManager>();
            if (player == null) return;

            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                player.playerHealth.TakeDamage(damage);
                damageTimer = 0f;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 플레이어가 트랩을 벗어나면 타이머 초기화
        if (collision.CompareTag("Player"))
        {
            damageTimer = 0f;
        }
    }
}
