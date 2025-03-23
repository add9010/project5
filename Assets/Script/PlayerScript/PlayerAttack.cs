using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    private PlayerManager manager;
    private float timeSinceAttack;
    private int attackCount;
    private bool isAttacking;
    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();

    private void Start()
    {
        manager = PlayerManager.Instance;
    }

    void Update()
    {
        timeSinceAttack += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && timeSinceAttack > manager.data.attackDuration && !isAttacking)
        {
            attackCount++;
            StartCoroutine(AttackCoroutine());
        }
    }

    private System.Collections.IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        hitEnemies.Clear();

        manager.animator.SetTrigger("Attack" + ((attackCount % 3) + 1));
        float elapsed = 0f;

        while (elapsed < manager.data.attackDuration)
        {
            Attack();
            elapsed += Time.deltaTime;
            yield return null;
        }

        isAttacking = false;
        timeSinceAttack = 0f;
    }

    private void Attack()
    {
        float knockback = (attackCount == 3) ? manager.data.attackKnockbackThird : manager.data.attackKnockback;
        float damage = (attackCount == 3) ? manager.data.attackPower * 1.5f : manager.data.attackPower;

        if (attackCount == 3)
        {
            manager.cameraShake.ShakeCamera();
            attackCount = 0;
        }

        Vector3 pos = manager.attackPos.position;
        if (manager.spriteRenderer.flipX)
            pos.x -= manager.data.attackBoxSize.x;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(pos, manager.data.attackBoxSize, 0);
        foreach (Collider2D col in colliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null && !hitEnemies.Contains(col))
            {
                ParameterPlayerAttack arg = new ParameterPlayerAttack
                {
                    damage = damage,
                    knockback = knockback
                };
                enemy.TakeDamage(arg);
                hitEnemies.Add(col);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (manager != null && manager.attackPos != null)
        {
            Gizmos.color = Color.red;
            Vector3 pos = manager.attackPos.position;
            if (manager.spriteRenderer.flipX)
                pos.x -= manager.data.attackBoxSize.x;
            Gizmos.DrawWireCube(pos, manager.data.attackBoxSize);
        }
    }
}
