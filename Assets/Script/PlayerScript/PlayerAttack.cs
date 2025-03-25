using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttack
{
    private PlayerManager manager;
    private float timeSinceAttack;
    private int attackCount;
    private bool isAttacking;
    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();

    public bool IsAttacking { get { return isAttacking; } }

    public PlayerAttack(PlayerManager manager)
    {
        this.manager = manager;
    }

    public void Update()
    {
        timeSinceAttack += Time.deltaTime;
    }

    public bool TryAttack()
    {
        bool mouseClicked = Input.GetMouseButtonDown(0);
        bool readyToAttack = timeSinceAttack >= manager.data.attackDuration;
        bool notCurrentlyAttacking = !isAttacking;

        return mouseClicked && readyToAttack && notCurrentlyAttacking;
    }

    public void DoAttack()
    {
        manager.StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        hitEnemies.Clear();

        attackCount++;
        if (attackCount >= 3)
        {
            attackCount = 0;
        }

        timeSinceAttack = 0f;

        // ���� �ִϸ��̼� Ʈ���� (1, 2, 3 ��ȯ)
        string animationTrigger = "Attack" + (attackCount + 1);
        manager.animator.SetTrigger(animationTrigger);

        // ���� �ִϸ��̼� ���� �ð� ���
        yield return new WaitForSeconds(manager.data.attackDuration / 2f);

        PerformAttack();

        // ������ �ִϸ��̼� �ð� ���
        yield return new WaitForSeconds(manager.data.attackDuration / 2f);

        isAttacking = false;
    }

    private void PerformAttack()
    {
        float knockback = (attackCount == 3) ? manager.data.attackKnockbackThird : manager.data.attackKnockback;
        float damage = (attackCount == 3) ? manager.data.attackPower * 1.5f : manager.data.attackPower;

        if (attackCount == 3)
            manager.cameraShake.ShakeCamera();

        Vector3 pos = manager.attackPos.position;
        if (manager.spriteRenderer.flipX)
            pos.x -= manager.data.attackBoxSize.x;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(pos, manager.data.attackBoxSize, 0);
        foreach (Collider2D col in colliders)
        {
            if (hitEnemies.Contains(col)) continue;

            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                var arg = new ParameterPlayerAttack { damage = damage, knockback = knockback };
                enemy.TakeDamage(arg);
                hitEnemies.Add(col);
            }
        }
    }
    public void UpdateAttackPosition()
    {
        Vector3 pos = manager.data.attackBoxOffset;
        if (manager.spriteRenderer.flipX)
            pos.x = -pos.x;

        manager.attackPos.localPosition = pos;
    }

    public void DrawGizmos()
    {
        if (manager == null || manager.attackPos == null) return;

        Gizmos.color = Color.cyan;
        Vector3 pos = manager.attackPos.position;
        Gizmos.DrawWireCube(pos, manager.data.attackBoxSize);
    }
}