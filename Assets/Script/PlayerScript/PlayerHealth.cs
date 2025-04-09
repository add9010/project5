using UnityEngine;

public class PlayerHealth : IDamageable, IKnockbackable
{
    private PlayerManager pm;
    public float currentHealth { get; private set; }
    private bool isDead = false;

    public PlayerHealth(PlayerManager manager)
    {
        this.pm = manager;
        currentHealth = manager.data.maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        pm.playerStateController.SetHurt();
        // pm.UpdateHpUI(currentHealth);

        if (currentHealth <= 0)
        {
            isDead = true;
            Die();
        }
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        pm.rb.linearVelocity = Vector2.zero;
        pm.rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    private void Die()
    {
        Debug.Log("플레이어 사망");
        pm.MarkAsDead();

        // 씬 리셋
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, pm.data.maxHealth);
        //pm.UpdateHpUI(currentHealth);
    }
}
