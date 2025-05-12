using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerAttack
{
    private PlayerManager pm;
    private float timeSinceAttack;
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
        UnityEngine.Debug.Log("AttackCoroutine 시작!");
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
        pm.GetAnimator().SetTrigger(animationTrigger);

        // 공격 애니메이션 절반 시간 대기
        yield return new WaitForSeconds(pm.data.attackDuration / 2f);

        PerformAttack();

        // 나머지 애니메이션 시간 대기
        yield return new WaitForSeconds(pm.data.attackDuration / 2f);

        isAttacking = false;
    }

    private void PerformAttack()
    {
        float knockback = (attackCount == 3) ? pm.data.attackKnockbackThird : pm.data.attackKnockback;
        float damage = (attackCount == 3) ? pm.data.attackPower * 1.5f : pm.data.attackPower;

        if (attackCount == 3)
            pm.cameraController.Shake(0.1f, 0.3f);

        Vector3 pos = pm.attackPos.position;

        int enemyLayerMask = LayerMask.GetMask("Enemy");
        Collider2D[] colliders = Physics2D.OverlapBoxAll(pos, pm.data.attackBoxSize, 0, enemyLayerMask);

        foreach (Collider2D col in colliders)
        {
            if (hitEnemies.Contains(col)) continue;

            GameObject target = col.gameObject;
            // 수정된 코드
            // NullReferenceException 방지를 위해 Instance가 null이 아니고, isConnected가 true일 때만 진입
            if (NetworkClient.Instance != null && NetworkClient.Instance.isConnected)
            {
                NetworkCombatManager.SendMonsterDamage((int)damage);
                Debug.Log($"데미지 전송: {damage}");
            }
            else
            {
                // 네트워크 연결이 없거나 Instance 자체가 없으면 로컬 처리
                CombatManager.ApplyDamage(target, damage, knockback, pm.transform.position);
            }
        }
    }

    public void UpdateAttackPosition()
    {
        Vector3 offset = pm.data.attackBoxOffset;

        // 반전된 스프라이트 방향 기준으로 x축 이동 반전
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
