using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class DamageDealer : MonoBehaviour
{
    private float damage;
    private float knockback;
    private float attackDuration;
    private int attackCount;
    private Transform target;  // 추적할 대상 (플레이어)
    private Vector2 boxSize;   // 공격 박스 크기
    private int currentAttack = 0;
    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();

    public void Initialize(float damage, float knockback, float attackDuration, int attackCount, Transform target, Vector2 boxSize)
    {
        this.damage = damage;
        this.knockback = knockback;
        this.attackDuration = attackDuration;
        this.attackCount = attackCount;
        this.target = target;
        this.boxSize = boxSize;

        // 즉시 첫 번째 데미지 적용
        DealDamage();

        // 코루틴 시작
        StartCoroutine(DealDamageCoroutine());
    }

    private void Update()
    {
        if (target != null)
        {
            // 대상의 위치를 따라 이동
            transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * 5f);
        }
    }

    private IEnumerator DealDamageCoroutine()
    {
        while (currentAttack < attackCount)
        {

            // 이후 공격 간격 대기
            currentAttack++;
            yield return new WaitForSeconds(attackDuration);
        }

        Destroy(gameObject); // 공격이 끝난 후 프리팹 제거
    }

    private void DealDamage()
    {
        // 적과 충돌 체크
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, boxSize, 0);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy") && !hitEnemies.Contains(collider))
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    // 데미지 및 넉백 처리
                    Vector2 knockbackDirection = (collider.transform.position - transform.position).normalized;
                    ParameterPlayerAttack attackParams = new ParameterPlayerAttack
                    {
                        damage = damage,
                        knockback = knockback,
                        knockbackDirection = knockbackDirection
                    };

                    enemy.TakeDamage(attackParams); // 적에게 데미지 및 스턴 적용
                    hitEnemies.Add(collider);
                }
            }
        }

        // 타격 대상 초기화
        hitEnemies.Clear();
    }

    private void OnDrawGizmos()
    {
        // 공격 범위를 시각화
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}
