using UnityEngine;

public class PlayerHealth
{
    private PlayerManager manager;
    public float currentHealth { get; private set; }
    private bool isDead = false;

    public PlayerHealth(PlayerManager manager)
    {
        this.manager = manager;
        currentHealth = manager.data.maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        manager.animator.SetTrigger("Hurt");
        manager.UpdateHpUI(currentHealth);

        if (currentHealth <= 0)
        {
            isDead = true;
            Die();
        }
    }

    private void Die()
    {
        manager.animator.SetTrigger("Death");
        Debug.Log("ÇÃ·¹ÀÌ¾î »ç¸Á");
        manager.MarkAsDead();

        // ¾À ¸®¼Â
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, manager.data.maxHealth);
        manager.UpdateHpUI(currentHealth);
    }
}