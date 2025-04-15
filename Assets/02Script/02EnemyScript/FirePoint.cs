using UnityEngine;

public class FirePoint : MonoBehaviour
{
    public float lifetime = 5f;
    public float damage = 10f;

    private void Start()
    {
        Destroy(gameObject, lifetime); // 자동 제거
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerManager player = collision.GetComponent<PlayerManager>();
            if (player != null && !player.IsDead)
            {
                player.playerHealth.TakeDamage(damage);
            }

            Destroy(gameObject); // 피격 후 제거
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject); // 땅에 닿아도 제거
        }
    }
}
