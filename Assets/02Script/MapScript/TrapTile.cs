using UnityEngine;
using System.Collections.Generic;

public class TrapTile : MonoBehaviour
{
    [Header("트랩 기본 설정")]
    public float damage = 10f;
    public float damageInterval = 2.5f;

    // 트랩 안에 있는 오브젝트별 타이머 관리
    private Dictionary<Collider2D, float> damageTimers = new();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerManager player = collision.GetComponent<PlayerManager>();
            if (player != null)
            {
                player.playerHealth.TakeDamage(damage);
                damageTimers[collision] = 0f;
            }
        }
        else if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                damageTimers[collision] = 0f;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!damageTimers.ContainsKey(collision)) return;

        damageTimers[collision] += Time.deltaTime;

        if (damageTimers[collision] >= damageInterval)
        {
            if (collision.CompareTag("Player"))
            {
                PlayerManager player = collision.GetComponent<PlayerManager>();
                if (player != null)
                {
                    player.playerHealth.TakeDamage(damage);
                }
            }
            else if (collision.CompareTag("Enemy"))
            {
                Enemy enemy = collision.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }

            damageTimers[collision] = 0f; // 타이머 초기화
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (damageTimers.ContainsKey(collision))
        {
            damageTimers.Remove(collision);
        }
    }
}
