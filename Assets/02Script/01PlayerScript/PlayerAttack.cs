using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttack
{
    private PlayerManager pm;
    private float timeSinceAttack;
    private float attackInputTimer = 0f;
    private const float attackResetDelay = 0.5f;

    private int attackCount;
    private bool isAttacking;
    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();

    public bool IsAttacking => isAttacking;

    public PlayerAttack(PlayerManager manager)
    {
        this.pm = manager;
    }

    public void Update()
    {
        timeSinceAttack += Time.deltaTime;

        // 입력 없을 경우 카운트 초기화 타이머
        if (!Input.GetKey(KeyCode.Z))
        {
            attackInputTimer += Time.deltaTime;
            if (attackInputTimer >= attackResetDelay)
            {
                attackCount = 0;
                attackInputTimer = 0f;
            }
        }
        else
        {
            attackInputTimer = 0f;
        }
    }

    public bool TryAttack()
    {
        bool zKeyPressed = Input.GetKeyDown(KeyCode.Z);
        bool readyToAttack = timeSinceAttack >= pm.data.attackDuration;
        bool notCurrentlyAttacking = !isAttacking;
        return zKeyPressed && readyToAttack && notCurrentlyAttacking;
    }

    public void DoAttack()
    {
        pm.StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        Debug.Log("AttackCoroutine 시작!");
        isAttacking = true;
        hitEnemies.Clear();

        timeSinceAttack = 0f;

        string animationTrigger = "Attack" + (attackCount + 1);
        pm.GetAnimator().SetTrigger(animationTrigger);

        attackCount++;
        if (attackCount >= 3)
            attackCount = 0;



        // ▶ 공격 중 전진 지속
        Vector2 direction = pm.spriteRenderer.flipX ? Vector2.left : Vector2.right;
        float elapsed = 0f;
        float moveDuration = pm.data.attackForwardDuration;

        while (elapsed < moveDuration)
        {
            pm.rb.linearVelocity = direction * pm.data.attackForwardSpeed;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // ▶ 정지
        pm.rb.linearVelocity = Vector2.zero;

        // ▶ 공격 판정
        PerformAttack();

        // ▶ 공격 애니메이션 잔여 시간 기다리기
        float remain = Mathf.Max(0f, pm.data.attackDuration - moveDuration);
        if (remain > 0f)
            yield return new WaitForSeconds(remain);

        isAttacking = false;
    }

    private void PerformAttack()
    {
        float knockback = (attackCount == 3) ? pm.data.attackKnockbackThird : pm.data.attackKnockback;
        float damage = (attackCount == 3) ? pm.data.attackPower * 1.5f : pm.data.attackPower;

        Vector3 pos = pm.attackPos.position;
        int enemyLayerMask = LayerMask.GetMask("Enemy");
        Collider2D[] colliders = Physics2D.OverlapBoxAll(pos, pm.data.attackBoxSize, 0, enemyLayerMask);

        bool hitSomething = false;

        foreach (Collider2D col in colliders)
        {
            if (hitEnemies.Contains(col)) continue;

            GameObject target = col.gameObject;
            hitSomething = true;

            if (NetworkClient.Instance != null && NetworkClient.Instance.isConnected)
            {
                NetworkCombatManager.SendMonsterDamage((int)damage);
                Debug.Log($"데미지 전송: {damage}");
            }
            else
            {
                CombatManager.ApplyDamage(target, damage, knockback, pm.transform.position);
            }

            hitEnemies.Add(col);
        }

        // ▶ 적을 맞췄을 때만 카메라 흔들림
        if (hitSomething)
        {
            if (attackCount == 3)
                pm.cameraController.Shake(0.1f, 0.3f);
            else
                pm.cameraController.Shake(0.05f, 0.2f);
        }
    }

    public void UpdateAttackPosition()
    {
        Vector3 offset = pm.data.attackBoxOffset;
        if (!pm.spriteRenderer.flipX)
            offset.x *= -1;

        pm.attackPos.localPosition = offset;
    }

    public void DrawGizmos()
    {
        if (pm == null || pm.attackPos == null) return;

        Gizmos.color = Color.cyan;
        Vector3 pos = pm.attackPos.position;
        Gizmos.DrawWireCube(pos, pm.data.attackBoxSize);
    }
}
