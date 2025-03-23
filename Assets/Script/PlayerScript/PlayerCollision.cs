using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private PlayerManager manager;

    private void Start()
    {
        manager = PlayerManager.Instance;
    }

    public void IgnoreEnemyCollisions(bool ignore)
    {
        Collider2D[] enemies = Physics2D.OverlapBoxAll(transform.position, manager.data.attackBoxSize, 0);
        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Collider2D playerCollider = GetComponent<Collider2D>();
                if (playerCollider != null)
                {
                    Physics2D.IgnoreCollision(playerCollider, enemy, ignore);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, manager != null ? manager.data.attackBoxSize : Vector2.one);
    }
}
