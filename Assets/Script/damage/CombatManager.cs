using UnityEngine;

public class CombetManager : MonoBehaviour
{
    public float maxHealth;
    protected float currentHealth;
    protected bool isDead = false;
    protected Rigidbody2D rb;
    protected Animator animator;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage, float knockback, Vector2 direction)
    {
        if (isDead) return;

        currentHealth -= damage;
        animator.SetTrigger("Hurt");

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * knockback, ForceMode2D.Impulse);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");
    }

    public bool IsDead() => isDead;
}