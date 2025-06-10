using UnityEngine;
using System.Collections;
using static UnityEngine.Object;
using UnityEngine.SceneManagement;

public class PlayerHealth : IDamageable, IKnockbackable
{
    private PlayerManager pm;
    public float currentHealth { get; private set; }
    private bool isDead = false;

    // 무적 관련
    private bool isInvincible = false;
    private float invincibleTime = 1f;
    private float invincibleTimer = 0f;

    public PlayerHealth(PlayerManager manager)
    {
        this.pm = manager;
        currentHealth = manager.data.maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        pm.UpdateHpUI(currentHealth);

        if (isDead || isInvincible) return;

        pm.playerStateController.SetHurt();

        // 무적 상태 진입
        isInvincible = true;
        invincibleTimer = invincibleTime;
        pm.StartCoroutine(FlashWhileInvincible());

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
        //Debug.Log("플레이어 사망");

        isInvincible = false;
        invincibleTimer = 0f;

        if (pm.spriteRenderer != null)
            pm.spriteRenderer.enabled = true;

        pm.MarkAsDead();
        pm.playerStateController.ForceSetDead();

        
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, pm.data.maxHealth);
        pm.UpdateHpUI(currentHealth);
    }

    public void Update() // Update는 PlayerManager에서 호출 필요
    {
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0f)
            {
                isInvincible = false;
                if (pm.spriteRenderer != null)
                    pm.spriteRenderer.enabled = true; // 깜빡임 복구
            }
        }
    }
    public void Reset()
    {
        currentHealth = pm.data.maxHealth;
        isDead = false;
        isInvincible = false;
        invincibleTimer = 0f;

        if (pm.spriteRenderer != null)
            pm.spriteRenderer.enabled = true;

        pm.UpdateHpUI(currentHealth);
    }
    public void ResetHealth()
    {
        currentHealth = pm.data.maxHealth;
        isDead = false;                // ✅ 이 라인 중요!
        isInvincible = false;
        invincibleTimer = 0f;

        pm.UpdateHpUI(currentHealth);

        if (pm.spriteRenderer != null)
            pm.spriteRenderer.enabled = true;

        //Debug.Log("[PlayerHealth] 상태 초기화됨");
    }
    private IEnumerator FlashWhileInvincible()
    {
        SpriteRenderer sr = pm.spriteRenderer;
        while (isInvincible)
        {
            if (sr != null)
            {
                sr.enabled = false;
                yield return new WaitForSeconds(0.1f);
                sr.enabled = true;
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                yield break;
            }
        }
    }
}
