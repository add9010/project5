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

    public bool IsAttacking => isAttacking;

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
        Debug.Log("AttackCoroutine 시작!");
        isAttacking = true;
        hitEnemies.Clear();

        attackCount++;
        if (attackCount >= 3)
        {
            attackCount = 0;
        }

        timeSinceAttack = 0f;

        // 공격 애니메이션 트리거 (1, 2, 3 순환)
        string animationTrigger = "Attack" + (attackCount + 1);
        manager.animator.SetTrigger(animationTrigger);

        // 공격 애니메이션 절반 시간 대기
        yield return new WaitForSeconds(manager.data.attackDuration / 2f);

        PerformAttack();

        // 나머지 애니메이션 시간 대기
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

        int enemyLayerMask = LayerMask.GetMask("Enemy");
        Collider2D[] colliders = Physics2D.OverlapBoxAll(pos, manager.data.attackBoxSize, 0, enemyLayerMask);

        foreach (Collider2D col in colliders)
        {
            if (hitEnemies.Contains(col)) continue;

            GameObject target = col.gameObject;
            CombatManager.ApplyDamage(target, damage, knockback, manager.transform.position);
            hitEnemies.Add(col);
        }
    }

    public void UpdateAttackPosition()
    {
        Vector3 offset = manager.data.attackBoxOffset;

        // 반전된 스프라이트 방향 기준으로 x축 이동 반전
        if (!manager.spriteRenderer.flipX)
            offset.x *= -1;

        manager.attackPos.localPosition = offset;
    }

    public void DrawGizmos()
    {
        if (manager == null || manager.attackPos == null) return;

        Gizmos.color = Color.cyan;
        Vector3 pos = manager.attackPos.position;
        Gizmos.DrawWireCube(pos, manager.data.attackBoxSize);
    }
}
