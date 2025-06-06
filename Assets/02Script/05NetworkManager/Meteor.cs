using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float damage = 15f;
    public float knockbackForce = 3f;

    private void Start()
    {
        Destroy(gameObject, 1.9f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CombatManager.ApplyDamage(collision.gameObject, damage, knockbackForce, transform.position);
            Destroy(gameObject); // 충돌 후 파괴
        }
    }
}
