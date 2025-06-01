using System.Collections;
using UnityEngine;

public class Wizard : Enemy
{
    [Header("Wizard Settings")]
    public GameObject projectilePrefab;    // 기존에 있던 발사체 프리팹
    public Transform firePoint;            // 발사 위치
    public GameObject chargeCirclePrefab;  // ★ 새로 추가: 차지 효과 프리팹
    private bool isCharging = false;
    protected override void Start()
    {
        base.Start();

        // Wizard 스탯 세팅
        SetEnemyStatus("마법사", 60, 15, 3);
        attackRange = 4f;
        attackCooldown = 5f;

        // Wizard만 3초 차지
        attackHitDelay = 3f;
    }

    // PerformAttack()가 호출되는 시점은 AttackState.Update()에서
    // "timer >= attackHitDelay"이 되었을 때 한 번만 실행됩니다.
    public override void PerformAttack()
    {
        if (isCharging) return;

        StartCoroutine(ChargeAndShoot());
    }

    private IEnumerator ChargeAndShoot()
    {
        isCharging = true;

        // 1) 플레이어 방향 구하기
        Vector2 dir = (player.position - firePoint.position).normalized;

        // 2) 차징 이펙트(ChargeCircle) 생성
        GameObject circle = null;
        if (chargeCirclePrefab != null && firePoint != null)
        {
            // 발사 위치 근처에 생성
            circle = Instantiate(chargeCirclePrefab, firePoint.position, Quaternion.identity);
            circle.transform.SetParent(transform, true);

            // 3) 차징 이펙트 스프라이트 뒤집기 (left 방향일 때만)
            SpriteRenderer sr = circle.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // dir.x < 0 => 적(마법사)이 플레이어보다 오른쪽에 있는 상황 (왼쪽 발사)
                // 그러면 스프라이트를 flipX = true 해서 좌우 반전
                sr.flipX = (dir.x > 0f);
            }
        }

        // 4) 3초 대기하는 동안 차징 유지
        float elapsed = 0f;
        while (elapsed < attackHitDelay)
        {
            if (currentState is StaggerState || currentState is DeadState)
            {
                if (circle != null) Destroy(circle);
                isCharging = false;
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 5) 차징 끝나면 효과 제거
        if (circle != null)
            Destroy(circle);

        // 6) 실제 투사체 발사
        if (player != null && projectilePrefab != null && firePoint != null)
        {
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            // 투사체도 같은 논리로 flip 처리 (생략 가능)
            SpriteRenderer srProj = proj.GetComponent<SpriteRenderer>();
            if (srProj != null)
                srProj.flipX = (dir.x > 0f);

            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = dir * 5f;

            Projectile pjComp = proj.GetComponent<Projectile>();
            if (pjComp != null)
                pjComp.damage = atkDmg;
        }

        RecordAttackTime();
        isCharging = false;
    }

}
