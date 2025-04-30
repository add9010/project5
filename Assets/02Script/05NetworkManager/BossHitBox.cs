using UnityEngine;

public class BossHitBox : MonoBehaviour
{
    public float damage = 20f;
    public float knockbackForce = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CombatManager.ApplyDamage(collision.gameObject, damage, knockbackForce, transform.position);
        }
    }
}
